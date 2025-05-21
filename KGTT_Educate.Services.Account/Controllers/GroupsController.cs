using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KGTT_Educate.Services.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IUoW _uow;

        public GroupsController(IUoW uow)
        {
            _uow = uow;
        }

        [HttpPost]
        public IActionResult CreateGroup(Group group)
        {
            if (group == null) return BadRequest();

            group.Id = Guid.NewGuid();

            _uow.Groups.Add(group);

            _uow.Save();

            return Ok(group.Adapt<GroupDTO>());
        }

        [HttpGet("User/{userId}")]
        public IActionResult GetUserGroup(Guid userId)
        {
            UserGroup userGroup = _uow.UserGroup.Get(x => x.UserId == userId, "User,Group");

            if (userGroup == null) return NotFound();

            return Ok(userGroup.Adapt<UserGroupDTO>());
        }

        [HttpGet("Users/{groupId}")]
        public IActionResult GetUsersByGroup(Guid groupId)
        {
            IEnumerable<UserGroup> users = _uow.UserGroup.GetMany(x => x.GroupId == groupId, "User,Group");

            if (users == null || users.Count() == 0) return NotFound();

            return Ok(users.Adapt<IEnumerable<UserGroupDTO>>());
        }

        [HttpPost("User/{userId}")]
        public IActionResult CreateUserGroup(Guid userId, [FromForm] Guid groupId)
        {
            User user = _uow.Users.Get(x => x.Id == userId);

            if (user == null) return NotFound("Пользователь не найден");

            Group group = _uow.Groups.Get(x => x.Id == groupId);

            if (group == null) return NotFound("Группа не найдена");

            UserGroup userGroup = new()
            {
                User = user,
                Group = group
            };

            _uow.UserGroup.Add(userGroup);
            _uow.Save();

            return Ok(userGroup.Adapt<UserGroupDTO>());
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteGroup(Guid id)
        {
            Group group = _uow.Groups.Get(x => x.Id == id);

            if (group == null) return NotFound();

            _uow.Groups.Delete(group);

            _uow.Save();

            return Ok(group.Adapt<GroupDTO>());
        }

        [HttpDelete("User/{userId}")]
        public IActionResult DeleteUserGroup(Guid userId)
        {
            UserGroup userGroup = _uow.UserGroup.Get(x => x.UserId == userId, "User,Group");

            _uow.UserGroup.Delete(userGroup);

            _uow.Save();

            return Ok(userGroup.Adapt<UserGroupDTO>());
        }
    }
}
