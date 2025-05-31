using KGTT_Educate.Services.Events.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Events.Models
{
    public class EventUser
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; }
    }
}
