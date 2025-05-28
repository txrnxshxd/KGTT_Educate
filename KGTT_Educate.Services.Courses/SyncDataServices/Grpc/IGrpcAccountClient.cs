using KGTT_Educate.Services.Courses.Models.Dto;

namespace KGTT_Educate.Services.Courses.SyncDataServices.Grpc
{
    public interface IGrpcAccountClient
    {
        Task<GroupDTO> GetGroupAsync(Guid id);
    }
}
