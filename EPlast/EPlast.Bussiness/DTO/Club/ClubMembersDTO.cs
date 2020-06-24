﻿using EPlast.BusinessLogicLayer.DTO.UserProfiles;
using System.Collections.Generic;

namespace EPlast.BusinessLogicLayer.DTO.Club
{
    public class ClubMembersDTO
    {
        public int ID { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
        public int ClubId { get; set; }
        public ClubDTO Club { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<ClubAdministrationDTO> ClubAdministration { get; set; }
    }
}