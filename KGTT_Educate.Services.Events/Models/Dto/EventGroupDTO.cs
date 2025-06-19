using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Events.Models.Dto
{
    public class EventGroupDTO
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Event Event { get; set; }
    }
}
