﻿using AutoMapper;
using EPlast.BLL.DTO.Admin;
using EPlast.BLL.DTO.City;
using EPlast.BLL.Interfaces.Admin;
using EPlast.BLL.Interfaces.City;
using EPlast.DataAccess.Entities;
using EPlast.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPlast.BLL.Services.City
{
    public class CityAdministrationService : ICityAdministrationService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly IAdminTypeService _adminTypeService;
        private readonly UserManager<User> _userManager;

        public CityAdministrationService(IRepositoryWrapper repositoryWrapper,
            IMapper mapper,
            IAdminTypeService adminTypeService,
            UserManager<User> userManager)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _adminTypeService = adminTypeService;
            _userManager = userManager;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CityAdministrationDTO>> GetAdministrationByIdAsync(int cityId)
        {
            var cityAdministration = await _repositoryWrapper.CityAdministration.GetAllAsync(
                predicate: x => x.CityId == cityId,
                include: x => x.Include(q => q.User).
                     Include(q => q.AdminType));

            return _mapper.Map<IEnumerable<CityAdministration>, IEnumerable<CityAdministrationDTO>>(cityAdministration);
        }

        /// <inheritdoc />
        public async Task<CityAdministrationDTO> AddAdministratorAsync(CityAdministrationDTO adminDTO)
        {
            var adminType = await _adminTypeService.GetAdminTypeByNameAsync(adminDTO.AdminType.AdminTypeName);
            var admin = new CityAdministration()
            {
                StartDate = adminDTO.StartDate ?? DateTime.Now,
                EndDate = adminDTO.EndDate,
                AdminTypeId = adminType.ID,
                CityId = adminDTO.CityId,
                UserId = adminDTO.UserId
            };

            var user = await _userManager.FindByIdAsync(adminDTO.UserId);
            var role = adminType.AdminTypeName == "Голова Станиці" ? "Голова Станиці" : "Діловод Станиці";
            await _userManager.AddToRoleAsync(user, role);

            _repositoryWrapper.CityAdministration.Attach(admin);
            await _repositoryWrapper.CityAdministration.CreateAsync(admin);
            await _repositoryWrapper.SaveAsync();

            return adminDTO;
        }

        /// <inheritdoc />
        public async Task<CityAdministrationDTO> EditAdministratorAsync(CityAdministrationDTO adminDTO)
        {
            var user = await _userManager.FindByIdAsync(adminDTO.UserId);
            var adminType = await _adminTypeService.GetAdminTypeByIdAsync(adminDTO.AdminTypeId);
            var role = adminType.AdminTypeName == "Голова Станиці" ? "Голова Станиці" : "Діловод Станиці";

            if (adminType.AdminTypeName != adminDTO.AdminType.AdminTypeName)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            adminType = await _adminTypeService.GetAdminTypeByNameAsync(adminDTO.AdminType.AdminTypeName);
            role = adminType.AdminTypeName == "Голова Станиці" ? "Голова Станиці" : "Діловод Станиці";
            await _userManager.AddToRoleAsync(user, role);
            
            var admin = await _repositoryWrapper.CityAdministration.GetFirstOrDefaultAsync(a => a.ID == adminDTO.ID);
            admin.StartDate = adminDTO.StartDate ?? DateTime.Now;
            admin.EndDate = adminDTO.EndDate;
            admin.AdminTypeId = adminType.ID;
            admin.AdminType = _mapper.Map<AdminTypeDTO, AdminType>(adminType);

            _repositoryWrapper.CityAdministration.Attach(admin);
            _repositoryWrapper.CityAdministration.Update(admin);
            await _repositoryWrapper.SaveAsync();

            return adminDTO;
        }

        /// <inheritdoc />
        public async Task RemoveAdministratorAsync(int adminId)
        {
            var admin = await _repositoryWrapper.CityAdministration.GetFirstOrDefaultAsync(u => u.ID == adminId);
            admin.EndDate = DateTime.Now;

            var adminType = await _adminTypeService.GetAdminTypeByIdAsync(admin.AdminTypeId);
            var user = await _userManager.FindByIdAsync(admin.UserId);
            var role = adminType.AdminTypeName == "Голова Станиці" ? "Голова Станиці" : "Діловод Станиці";
            await _userManager.RemoveFromRoleAsync(user, role);

            _repositoryWrapper.CityAdministration.Update(admin);
            await _repositoryWrapper.SaveAsync();
        }
    }
}