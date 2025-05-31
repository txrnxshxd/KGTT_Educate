using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using KGTT_Educate.Services.Account.SyncDataServices.Http;
using KGTT_Educate.Services.Account.Utils;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json;

namespace KGTT_Educate.Services.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUoW _uow;
        private readonly ICommandDataClient _httpCommand;

        public AccountController(IUoW uow, ICommandDataClient httpCommand)
        {
            _uow = uow;
            _httpCommand = httpCommand;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<User> users = _uow.Users.GetAll();

            return Ok(users.Adapt<IEnumerable<Models.Dto.UserDTO>>());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            User user = _uow.Users.Get(x => x.Id == id);

            if (user == null) return NotFound();

            return Ok(user.Adapt<UserDTO>());
        }

        [HttpGet("Group/{groupId}")]
        public IActionResult GetByGroupId(Guid groupId)
        {
            IEnumerable<UserGroup> userGroup = _uow.UserGroup.GetMany(x => x.GroupId == groupId, "User,Group");

            if (userGroup.Count() == 0) return NotFound();

            return Ok(userGroup.Adapt<IEnumerable<Models.Dto.UserGroupDTO>>());
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromForm] UserRequest userRequest)
        {
            if (userRequest == null) return BadRequest();

            Hasher hasher = new();

            userRequest.Password = hasher.HashSHA512(userRequest.Password);

            User user = userRequest.Adapt<User>();
            user.CreatedAt = DateTime.UtcNow.Date;

            if (userRequest.FormFile != null)
            {
                try
                {
                    Console.WriteLine("--> Запрос к FilesAPI");
                    using HttpResponseMessage response = await _httpCommand.SendFile(userRequest.FormFile, "Accounts");

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Ошибка загрузки файла: {error}");
                    }

                    FilesApiResponse result = await response.Content.ReadFromJsonAsync<FilesApiResponse>();

                    user.AvatarLocalPath = result.LocalFilePath;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка сети: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Ошибка JSON: {ex.Message}"); ;
                }
            }
            else
            {
                user.AvatarLocalPath = null;
            }

            try
            {
                _uow.Users.Add(user);
                await _uow.SaveAsync();
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
                        case "IX_Users_PhoneNumber":
                            return BadRequest("Номер уже существует");
                        default:
                            return BadRequest("Дубликат данных");
                    }
                }
            }

            return Ok(user.Adapt<Models.Dto.UserDTO>());

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

            return Ok(user.Adapt<Models.Dto.UserDTO>());
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            User user = _uow.Users.Get(x => x.Id == id);

            if (user == null) return NotFound();

            _uow.Users.Delete(user);

            _uow.Save();

            return Ok(user.Adapt<Models.Dto.UserDTO>());
        }
    }
}
