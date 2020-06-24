﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EPlast.BusinessLogicLayer.DTO.Admin
{
    public class AdminTypeDTO
    {
        public int ID { get; set; }
        public string AdminTypeName { get; set; }
        public ICollection<CityAdministrationDTO> CityAdministration { get; set; }
        public ICollection<ClubAdministrationDTO> ClubAdministration { get; set; }
        public ICollection<RegionAdministrationDTO> RegionAdministration { get; set; }
    }
}
