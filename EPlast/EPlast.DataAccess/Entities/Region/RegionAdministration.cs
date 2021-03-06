﻿using System;
using System.ComponentModel.DataAnnotations;

namespace EPlast.DataAccess.Entities
{
    public class RegionAdministration
    {
        public int ID { get; set; }

        public int AdminTypeId { get; set; }
        public AdminType AdminType { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }
    }
}