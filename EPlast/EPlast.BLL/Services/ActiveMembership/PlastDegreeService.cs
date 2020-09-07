using AutoMapper;
using EPlast.BLL.DTO.ActiveMembership;
using EPlast.BLL.Interfaces.ActiveMembership;
using EPlast.BLL.Services.Interfaces;
using EPlast.DataAccess.Entities;
using EPlast.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPlast.BLL.Services.ActiveMembership
{
    /// <inheritdoc />
    public class PlastDegreeService : IPlastDegreeService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IUserManagerService _userManagerService;
        private readonly string _seniorPlatoonSupporterDegreeName;
        private readonly int _ageForSeniorPlatoonSupporterDegree;
        public PlastDegreeService(IMapper mapper, IRepositoryWrapper repoWrapper, IUserManagerService userManagerService)
        {
            _mapper = mapper;
            _repoWrapper = repoWrapper;
            _userManagerService = userManagerService;
            _seniorPlatoonSupporterDegreeName = "Старший пластун прихильник/Старша пластунка прихильниця";
            _ageForSeniorPlatoonSupporterDegree = 35;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PlastDegreeDTO>> GetDergeesAsync()
        {
            var degrees = await _repoWrapper.PlastDegrees.GetAllAsync();

            return _mapper.Map<IEnumerable<PlastDegreeDTO>>(degrees);
        }

        /// <inheritdoc />
        public async Task<DateTime> GetDateOfEntryAsync(string userId)
        {
            var userDTO = await _userManagerService.FindByIdAsync(userId);

            return userDTO.RegistredOn;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserPlastDegreeDTO>> GetUserPlastDegreesAsync(string userId)
        {
            var userPlastDegrees = await _repoWrapper.UserPlastDegrees.GetAllAsync(upd => upd.UserId == userId, include: pd => pd.Include(d => d.PlastDegree));

            return _mapper.Map<IEnumerable<UserPlastDegreeDTO>>(userPlastDegrees);
        }

        /// <inheritdoc />
        public async Task<bool> AddPlastDegreeForUserAsync(UserPlastDegreePostDTO userPlastDegreePostDTO)
        {
            bool isAdded = false;
            var userDto = await _userManagerService.FindByIdAsync(userPlastDegreePostDTO.UserId);
            if (userDto != null)
            {
                List<UserPlastDegree> userPlastDegrees = _mapper.Map<IEnumerable<UserPlastDegree>>(userDto.UserPlastDegrees).ToList();
                if (!userPlastDegrees.Any(upd => upd.PlastDegree.Id == userPlastDegreePostDTO.PlastDegreeId))
                {
                    UserPlastDegree userPlastDegree = _mapper.Map<UserPlastDegree>(userPlastDegreePostDTO);
                    PlastDegree plastDegree = await _repoWrapper.PlastDegrees.GetFirstOrDefaultAsync(pd => pd.Id == userPlastDegreePostDTO.PlastDegreeId);
                    if (plastDegree != null)
                    {
                        userPlastDegree.PlastDegree = plastDegree;
                        await SetDegreeAsCurrent(userPlastDegree.IsCurrent);
                        _repoWrapper.UserPlastDegrees.Attach(userPlastDegree);
                        _repoWrapper.UserPlastDegrees.Create(userPlastDegree);
                        await _repoWrapper.SaveAsync();
                        isAdded = true;
                    }

                }
            }

            return isAdded;
        }

        private async Task SetDegreeAsCurrent(bool IsUserPlastDegreeCurrent)
        {
            if (IsUserPlastDegreeCurrent)
            {
                UserPlastDegree prevCurrentUserPlastDegree = await _repoWrapper.UserPlastDegrees.GetFirstOrDefaultAsync(upd => upd.IsCurrent);
                if (prevCurrentUserPlastDegree != null)
                {
                    prevCurrentUserPlastDegree.IsCurrent = false;
                    _repoWrapper.UserPlastDegrees.Update(prevCurrentUserPlastDegree);
                    await _repoWrapper.SaveAsync();
                }

            }
        }

        /// <inheritdoc />
        public async Task<bool> DeletePlastDegreeForUserAsync(string userId, int plastDegreeId)
        {
            bool isDeleted = false;
            UserPlastDegree userPlastDegree = await _repoWrapper.UserPlastDegrees
                .GetFirstOrDefaultAsync(upd => upd.PlastDegreeId == plastDegreeId && upd.UserId == userId);
            if (userPlastDegree != null)
            {
                _repoWrapper.UserPlastDegrees.Delete(userPlastDegree);
                await _repoWrapper.SaveAsync();
                isDeleted = true;
            }

            return isDeleted;
        }

        /// <inheritdoc />
        public async Task<bool> AddEndDateForUserPlastDegreeAsync(UserPlastDegreePutDTO userPlastDegreePutDTO)
        {
            bool isAdded = false;
            UserPlastDegree userPlastDegree = await _repoWrapper.UserPlastDegrees
                .GetFirstOrDefaultAsync(upd => upd.PlastDegreeId == userPlastDegreePutDTO.PlastDegreeId && upd.UserId == userPlastDegreePutDTO.UserId);
            if (userPlastDegree != null)
            {
                userPlastDegree.DateFinish = userPlastDegreePutDTO.EndDate;
                _repoWrapper.UserPlastDegrees.Update(userPlastDegree);
                await _repoWrapper.SaveAsync();
                isAdded = true;
            }

            return isAdded;
        }

        /// <inheritdoc />
        public async Task<bool> SetPlastDegreeForUserAsCurrentAsync(string userId, int plastDegreeId)
        {
            bool isAdded = false;
            UserPlastDegree userPlastDegree = await _repoWrapper.UserPlastDegrees
               .GetFirstOrDefaultAsync(upd => upd.PlastDegreeId == plastDegreeId && upd.UserId == userId);
            if (userPlastDegree != null)
            {
                await SetDegreeAsCurrent(true);
                userPlastDegree.IsCurrent = true;
                _repoWrapper.UserPlastDegrees.Update(userPlastDegree);
                await _repoWrapper.SaveAsync();
                isAdded = true;
            }

            return isAdded;
        }

        public async Task AddSeniorPlastSupporterDegreeForAllUsersAsync()
        {
            var users = await _repoWrapper.UserProfile.GetAllAsync();
            var u = users.Where(up => IsMoreThan(up.Birthday, _ageForSeniorPlatoonSupporterDegree));
            var degreeToSet = await _repoWrapper.PlastDegrees.GetFirstOrDefaultAsync(pd => pd.Name == _seniorPlatoonSupporterDegreeName);
            var uniqueUserId = users.Select(u => u.UserID).Distinct();
            uniqueUserId.ToList().ForEach(async userId =>
            {
               // if (!await IsUserHasDegree(userId, degreeToSet.Id))
               // {
                    UserPlastDegree userPlastDegree = new UserPlastDegree
                    {
                        UserId = userId,
                        DateStart = DateTime.Now,
                        PlastDegreeId = degreeToSet.Id,
                        PlastDegree = degreeToSet

                    };
                    
                    //_repoWrapper.UserPlastDegrees.Attach(userPlastDegree);
                    _repoWrapper.UserPlastDegrees.Create(userPlastDegree);
                    await _repoWrapper.SaveAsync();
           //     }
            });
        }

        private bool IsMoreThan(DateTime? birthday, int years)
        {
            if (birthday == null)
                return false;
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan? span = DateTime.Now - birthday;

            return ((zeroTime + span)?.Year - 1) >= years;

        }
        private async Task<bool> IsUserHasDegree(string userId, int plastDegreeId)
        {
            UserPlastDegree userPlastDegree = await _repoWrapper.UserPlastDegrees
                 .GetFirstOrDefaultAsync(upd => upd.PlastDegreeId == plastDegreeId && upd.UserId == userId);

            return userPlastDegree != null;
        }
    }
}
