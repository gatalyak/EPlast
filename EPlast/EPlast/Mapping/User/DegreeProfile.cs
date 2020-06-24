﻿using AutoMapper;
using EPlast.BusinessLogicLayer.DTO.UserProfiles;
using EPlast.DataAccess.Entities;
using EPlast.ViewModels.UserInformation.UserProfile;

namespace EPlast.Mapping
{
    public class DegreeProfile:Profile
    {
        public DegreeProfile()
        {
            CreateMap<Degree, DegreeDTO>().ReverseMap();
            CreateMap<DegreeViewModel, DegreeDTO>().ReverseMap();
        }
    }
}
