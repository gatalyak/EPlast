﻿using AutoMapper;
using EPlast.BusinessLogicLayer.DTO.Events;
using EPlast.ViewModels.Events;

namespace EPlast.Mapping.Events
{
    public class GeneralEventInfoProfile : Profile
    {
        public GeneralEventInfoProfile()
        {
            CreateMap<GeneralEventDTO, GeneralEventViewModel>();
        }
    }
}
