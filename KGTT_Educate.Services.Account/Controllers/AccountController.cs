using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using KGTT_Educate.Services.Account.Utils;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace KGTT_Educate.Services.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUoW _uow;

        public AccountController(IUoW uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<User> users = _uow.Users.GetAll();

            return Ok(users.Adapt<IEnumerable<UserDTO>>());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            User user = _uow.Users.Get(x => x.Id == id);

            if (user == null) return NotFound();

            return Ok(user.Adapt<UserDTO>());
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (user == null) return BadRequest();

            Hasher hasher = new();

            user.Password = hasher.HashSHA512(user.Password);
            user.CreatedAt = DateTime.UtcNow.Date;

            try
            {
                _uow.Users.Add(user);
                _uow.Save();
            }
            catch (DbUpdateException ex)
            {
                // Ошибка дубликата
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    switch (pgEx.ConstraintName)
                    {
                        case "IX_Users_Login":
                            return BadRequest("Логин занят");
                        case "IX_Users_Email":
                            return BadRequest("Email уже используется");
                        case "IX_Users_Telegram":
                            return BadRequest("Telegram уже привязан");
                        default:
                            return BadRequest("Дубликат данных");
                    }
                }
            }

            return Ok(user.Adapt<UserDTO>());
        }

        [HttpPut]
        public IActionResult Update([FromBody] User user)
        {
            if (user == null) return BadRequest();

            try
            {
                _uow.Users.Update(user);
                _uow.Save();
            }
            catch (DbUpdateException ex)
            {
                // Ошибка дубликата
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
                {
                    switch (pgEx.ConstraintName)
                    {
                        case "IX_Users_Login":
                            return BadRequest("Логин занят");
                        case "IX_Users_Email":
                            return BadRequest("Email уже используется");
                        case "IX_Users_Telegram":
                            return BadRequest("Telegram уже привязан");
                        default:
                            return BadRequest("Дубликат данных");
                    }
                }
            }

            return Ok(user.Adapt<UserDTO>());
        }

        [HttpPut("Avatar/{userId}")]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateAvatar(IFormFile avatar, Guid userId)
        {
            if (avatar == null || userId == Guid.Empty) return BadRequest();

            User user = _uow.Users.Get(x => x.Id == userId);

            if (user == null) return NotFound();

            // Запрос к FilesAPI, после записываем пути к файлу в user

            return Ok(user.Adapt<UserDTO>());
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            User user = _uow.Users.Get(x => x.Id == id);

            if (user == null) return NotFound();

            _uow.Users.Delete(user);

            _uow.Save();

            return Ok(user.Adapt<UserDTO>());
        }
    }
}
