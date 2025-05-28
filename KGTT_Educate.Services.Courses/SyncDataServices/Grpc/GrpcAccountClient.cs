using Grpc.Net.Client;
using KGTT_Educate.Services.Account;
using Mapster;

namespace KGTT_Educate.Services.Courses.SyncDataServices.Grpc
{
    public class GrpcAccountClient : IGrpcAccountClient
    {
        private readonly IConfiguration _configuration;

        public GrpcAccountClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Models.Dto.GroupDTO> GetGroupAsync(Guid id)
        {
            Console.WriteLine($"--> Вызов gRpc сервиса {_configuration["GrpcAccount"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcAccount"]);
            var client = new UserService.UserServiceClient(channel);
            var request = new GetGroupRequest 
            {
                Id = id.ToString() 
            };

            try
            {
                var reply = await client.GetGroupAsync(request);
                return reply.Group.Adapt<Models.Dto.GroupDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Не удается вызвать gRpc Сервер: {ex.Message}");
                return null;
            }
        }
    }
}
