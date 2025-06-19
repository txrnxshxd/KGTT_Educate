using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace KGTT_Educate.Services.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUoW _uow;

        public RolesController(IUoW uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Role> roles = _uow.Roles.GetAll();

            if (roles.Count() == 0) return NotFound("Нет ни одной роли");

            return Ok(roles);
        }

        [HttpPost]
        public IActionResult CreateRole(Role role)
        {
            if (role == null) return BadRequest();

            _uow.Roles.Add(role);

            _uow.Save();

            return Ok(role);
        }

        [HttpGet("User/{userId}")]
        public IActionResult GetUserRoles(Guid userId)
        {
            IEnumerable<UserRole> userRole = _uow.UserRole.GetMany(x => x.UserId == userId, "User,Role");

            if (userRole == null) return NotFound();

            return Ok(userRole.Adapt<IEnumerable<UserRoleDTO>>());
        }

        [HttpGet("Users/{roleId}")]
        public IActionResult GetUsersByRole(Guid roleId)
        {
            IEnumerable<UserRole> users = _uow.UserRole.GetMany(x => x.RoleId == roleId, "User,Role");

            if (users == null || users.Count() == 0) return NotFound();

            return Ok(users.Adapt<IEnumerable<UserRoleDTO>>());
        }

        [HttpPost("User/{userId}")]
        public IActionResult CreateUserRole(Guid userId, [FromForm] Guid roleId)
        {
            User user = _uow.Users.Get(x => x.Id == userId);

            if (user == null) return NotFound();

            Role role = _uow.Roles.Get(x => x.Id == roleId);

            if (role == null) return NotFound();

            UserRole userRole = new()
            {
                User = user,
                Role = role
            };

            _uow.UserRole.Add(userRole);
            _uow.Save();

            return Ok(userRole.Adapt<UserRoleDTO>());
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRole(Guid id)
        {
            Role role = _uow.Roles.Get(x => x.Id == id);

            if (role == null) return NotFound();

            _uow.Roles.Delete(role);

            _uow.Save();

            return Ok(role);
        }

        [HttpDelete("User/{userId:guid}/Role/{roleId:guid}")]
        public IActionResult DeleteUserRole(Guid userId, Guid roleId)
        {
            UserRole userRole = _uow.UserRole.Get(x => x.UserId == userId && x.RoleId == roleId, "User,Role");

            _uow.UserRole.Delete(userRole);

            _uow.Save();

            return Ok(userRole.Adapt<UserRoleDTO>());
        }
    }
}
