﻿using EPlast.BLL.DTO.EventCalendar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPlast.BLL.Interfaces.EventUser
{
    public interface IEventsManager
    {
        Task<List<EventCalendarInfoDTO>> GetActionsAsync();

        Task<List<EventCalendarInfoDTO>> GetEducationsAsync();

        Task<List<EventCalendarInfoDTO>> GetCampsAsync();
    }
}
