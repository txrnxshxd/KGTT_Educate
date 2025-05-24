using KGTT_Educate.Services.Events.Models;
using KGTT_Educate.Services.Events.Models.Dto;

namespace KGTT_Educate.Services.Events.SyncDataServices.Grpc
{
    public interface IGrpcAccountClient
    {
        IEnumerable<UserGroupDTO> GetUserGroup(Guid groupId);
    }
}
