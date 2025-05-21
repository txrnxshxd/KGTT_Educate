using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Account.Models
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
