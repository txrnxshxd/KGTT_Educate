﻿using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Account.Models.Dto
{
    public class UserDTO
    {
        [Key]
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Telegram { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string? AvatarLocalPath { get; set; } = string.Empty;
    }
}
