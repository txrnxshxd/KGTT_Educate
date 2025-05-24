using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Events.Models
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
        [Required]
        public string Password { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? Telegram { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string? AvatarLocalPath { get; set; }
        public string? AvatarFullPath { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
