using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Events.Models.Dto
{
    public class EventGroupDTO
    {
        public GroupDTO Group { get; set; }
        public Event Event { get; set; }
    }
}
