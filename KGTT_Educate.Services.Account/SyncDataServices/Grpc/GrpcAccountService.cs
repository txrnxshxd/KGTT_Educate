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

            IEnumerable<UserGroupDTO> users = _uow.UserGroup.GetMany(x => x.GroupId == groupId, "User,Group").Adapt<IEnumerable<UserGroupDTO>>();

            foreach (var user in users)
            {
                response.UserGroup.Add(user);
            }

            return Task.FromResult(response);
        }

        public override Task<GetGroupResponse> GetGroup(GetGroupRequest request, ServerCallContext context)
        {
            var response = new GetGroupResponse();

            Guid Id = Guid.Parse(request.Id);

            GroupDTO group = _uow.Groups.Get(x => x.Id == Id).Adapt<GroupDTO>();
            response.Group = group;

            return Task.FromResult(response);
        }
    }
}
