using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using Mapster;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAll()
        {
            IEnumerable<Group> groups = _uow.Groups.GetAll();

            if (groups.Count() == 0) return NotFound("Нет ни одной группы");

            return Ok(groups);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Authenticated")]
        public IActionResult Get(Guid id)
        {
            Group group = _uow.Groups.Get(x => x.Id == id);

            return Ok(group);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateGroup(Group group)
        {
            if (group == null) return BadRequest();

            _uow.Groups.Add(group);

            _uow.Save();

            return Ok(group.Adapt<GroupDTO>());
        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Update(Group group)
        {
            if (group == null) return BadRequest();

            _uow.Groups.Update(group);

            _uow.Save();

            return Ok(group.Adapt<GroupDTO>());
        }

        [HttpGet("User/{userId}")]
        [Authorize(Policy = "Authenticated")]
        public IActionResult GetUserGroup(Guid userId)
        {
            IEnumerable<UserGroup> userGroup = _uow.UserGroup.GetMany(x => x.UserId == userId, "User,Group");

            if (userGroup == null) return NotFound();

            return Ok(userGroup.Adapt<IEnumerable<UserGroupDTO>>());
        }

        [HttpGet("Users/{groupId}")]
        [Authorize(Policy = "Authenticated")]
        public IActionResult GetUsersByGroup(Guid groupId)
        {
            IEnumerable<UserGroup> users = _uow.UserGroup.GetMany(x => x.GroupId == groupId, "User,Group");

            if (users == null || users.Count() == 0) return NotFound();

            return Ok(users.Adapt<IEnumerable<UserGroupDTO>>());
        }

        [HttpPost("User/{userId}")]
        [Authorize(Policy = "AdminOnly")]
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
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteGroup(Guid id)
        {
            Group group = _uow.Groups.Get(x => x.Id == id);

            if (group == null) return NotFound();

            _uow.Groups.Delete(group);

            _uow.Save();

            return Ok(group.Adapt<GroupDTO>());
        }

        [HttpDelete("User/{userId}/Group/{groupId}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteUserGroup(Guid userId, Guid groupId)
        {
            UserGroup userGroup = _uow.UserGroup.Get(x => x.UserId == userId && x.GroupId == groupId, "User,Group");

            _uow.UserGroup.Delete(userGroup);

            _uow.Save();

            return Ok(userGroup.Adapt<UserGroupDTO>());
        }
    }
}
