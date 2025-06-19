using Grpc.Net.Client;
using KGTT_Educate.Services.Events.Models;
using KGTT_Educate.Services.Events.Models.Dto;
using KGTT_Educate.Services.Account;
using Mapster;
using Grpc.Core;

namespace KGTT_Educate.Services.Events.SyncDataServices.Grpc
{
    public class GrpcAccountClient : IGrpcAccountClient
    {
        private readonly IConfiguration _configuration;

        public GrpcAccountClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Models.Dto.UserGroupDTO> GetUserGroup(Guid groupId)
        {
            Console.WriteLine($"--> Вызов gRpc сервиса {_configuration["GrpcAccount"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcAccount"]);
            var client = new UserService.UserServiceClient(channel);
            var request = new GetUserGroupRequest
            { 
                GroupId = groupId.ToString()
            };

            try
            {
                var reply = client.GetUserGroup(request);
                return reply.UserGroup.Adapt<IEnumerable<Models.Dto.UserGroupDTO>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Не удается вызвать gRpc Сервер: {ex.Message}");
                return null;
            }
        }

        public Models.Dto.GroupDTO GetGroup(Guid groupId)
        {
            Console.WriteLine($"--> Вызов gRpc сервиса {_configuration["GrpcAccount"]}");
            var channel = GrpcChannel.ForAddress(_configuration["GrpcAccount"]);
            var client = new UserService.UserServiceClient(channel);
            var request = new GetGroupRequest
            {
                Id = groupId.ToString()
            };

            try
            {
                var reply = client.GetGroup(request);
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
