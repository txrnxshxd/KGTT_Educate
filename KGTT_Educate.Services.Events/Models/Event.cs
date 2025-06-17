using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Events.Models
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? EventLocation { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateTime? Date { get; set; }
    }
}
