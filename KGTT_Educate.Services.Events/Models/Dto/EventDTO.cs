using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Events.Models.Dto
{
    public class EventDTO
    {
        public string? Name { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? EventLocation { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateTime? Date { get; set; }
    }
}
