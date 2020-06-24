﻿using EPlast.BusinessLogicLayer.DTO;
using EPlast.BusinessLogicLayer.Interfaces;
using System.Threading.Tasks;

namespace EPlast.BusinessLogicLayer
{
    public class HomeService : IHomeService
    {
        private readonly IEmailConfirmation _emailConfirmation;

        public HomeService(IEmailConfirmation emailConfirmation)
        {
            _emailConfirmation = emailConfirmation;
        }

        public Task SendEmailAdmin(ContactDTO contactDTO)
        {
            return _emailConfirmation.SendEmailAsync("eplastdmnstrtr@gmail.com",
                "Питання користувачів",
                $"Контактні дані користувача : Електронна пошта {contactDTO.Email}, " +
                $"Ім'я {contactDTO.Name}," +
                $"Телефон {contactDTO.PhoneNumber}  " +
                $"Опис питання : {contactDTO.FeedBackDescription}",
                contactDTO.Email);
        }
    }
}
