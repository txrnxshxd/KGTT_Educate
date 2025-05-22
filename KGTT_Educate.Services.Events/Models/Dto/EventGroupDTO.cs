using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Events.Models.Dto
{
    public class EventGroupDTO
    {
        [Key]
        public Guid Id { get; set; }
        public Group Group { get; set; }
        public Event Event { get; set; }
    }
}
