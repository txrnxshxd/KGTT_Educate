using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Events.Models
{
    [NotMapped]
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
