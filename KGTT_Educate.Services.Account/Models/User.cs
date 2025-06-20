﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Account.Models
{
    [Index(nameof(Login), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    [Index(nameof(Telegram), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Phone]
        [Length(10,10)]
        public string? PhoneNumber { get; set; }
        public string? Telegram { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string? AvatarLocalPath { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
