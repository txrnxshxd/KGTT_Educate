using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Education.Services.Account.Models
{
    public class UserGroup
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public int GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }
    }
}
