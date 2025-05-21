using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KGTT_Educate.Services.Account.Models.Dto
{
    public class UserGroupDTO
    {
        public UserDTO? User { get; set; }
        public GroupDTO? Group { get; set; }
    }
}
