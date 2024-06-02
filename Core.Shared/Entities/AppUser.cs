﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Shared.Entities
{
    public class AppUser : IdentityUser
    {
        [MaxLength(50)]
        public string? Name { get; set; } = null!;
        public string? Language { get; set; } = "English";
        public string? CountryCode { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserAvatar { get; set; }
        public long BalanceToken { get; set;}
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
