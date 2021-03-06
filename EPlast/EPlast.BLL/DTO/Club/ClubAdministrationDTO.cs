﻿using System;
using EPlast.BLL.DTO.Admin;

namespace EPlast.BLL.DTO.Club
{
    public class ClubAdministrationDTO
    {
        public int ID { get; set; }
        public int AdminTypeId { get; set; }
        public string AdminTypeName { get; set; }
        public AdminTypeDTO AdminType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ClubId { get; set; }
        public ClubDTO Club { get; set; }
        public ClubMembersDTO ClubMembers { get; set; }
        public int ClubMembersID { get; set; }
    }
}