using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Events.Models.Dto
{
    public class EventUserDTO
    {
        [Key]
        public Guid Id { get; set; }
        public UserDTO User { get; set; }
        public Event Event { get; set; }
    }
}
