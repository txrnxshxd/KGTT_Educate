using Grpc.Core;
using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using Mapster;

namespace KGTT_Educate.Services.Account.SyncDataServices.Grpc
{
    public class GrpcAccountService : UserService.UserServiceBase
    {
        private readonly IUoW _uow;

        public GrpcAccountService(IUoW uow)
        {
            _uow = uow;
        }

        public override Task<GetUserGroupResponse> GetUserGroup(GetUserGroupRequest request, ServerCallContext context)
        {
            var response = new GetUserGroupResponse();

            Guid groupId = Guid.Parse(request.GroupId);

            IEnumerable<UserGroup> users = _uow.UserGroup.GetMany(x => x.GroupId == groupId, "User,Group");
            IEnumerable<UserGroupDTO> usersDTO = users.Adapt<IEnumerable<UserGroupDTO>>();

            foreach (var user in usersDTO)
            {
                response.UserGroup.Add(user);
            }

            return Task.FromResult(response);
        }
    }
}
