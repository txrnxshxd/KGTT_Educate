using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Account.Models
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public Guid RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }

    }
}
