using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Account.Models
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly AdmissionDate { get; set; }
        public DateOnly GraduationDate { get; set; }
    }
}
