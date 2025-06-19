using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.RequestResponseModels.Request;
using KGTT_Educate.Services.Account.Models.RequestResponseModels.Response;
using KGTT_Educate.Services.Account.SyncDataServices.Http;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        [Authorize(Policy = "Authenticated")]
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

            return Ok(user.Adapt<Models.Dto.UserDTO>());
        }

        [HttpGet("Group/{groupId}")]
        public IActionResult GetByGroupId(Guid groupId)
        {
            IEnumerable<UserGroup> userGroup = _uow.UserGroup.GetMany(x => x.GroupId == groupId, "User,Group");

            if (userGroup.Count() == 0) return NotFound();

            return Ok(userGroup.Adapt<IEnumerable<Models.Dto.UserGroupDTO>>());
        }

        [HttpPut]
        public IActionResult Update([FromBody] User user)
        {
            if (user == null) return BadRequest();

            User existingUser = _uow.Users.Get(x => x.Id == user.Id);

            if (existingUser == null) return NotFound();

            try
            {
                existingUser.Login = user.Login;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Telegram = user.Telegram;
                existingUser.LastName = user.LastName;
                existingUser.FirstName = user.FirstName;
                existingUser.MiddleName = user.MiddleName;

                if (!string.IsNullOrEmpty(user.Password))
                {
                    if (user.Password.Length < 8) return BadRequest("Пароль должен содержать минимум 8 символов");
                    existingUser.Password = new PasswordHasher<User>().HashPassword(user, user.Password);
                }

                _uow.Users.Update(existingUser);
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

            return Ok(user.Adapt<Models.Dto.UserDTO>());
        }

        [HttpPut("Avatar/{userId}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateAvatar(IFormFile avatar, Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();

            User user = _uow.Users.Get(x => x.Id == userId);

            if (user == null) return NotFound();

            if (avatar != null)
            {
                try
                {

                    using HttpResponseMessage uploadResponse = await _httpCommand.SendFile(avatar, "Accounts");

                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        var error = await uploadResponse.Content.ReadAsStringAsync();
                        return StatusCode(500, $"---> Ошибка загрузки файла: {error}");
                    }

                    if (user.AvatarLocalPath != null)
                    {
                        using HttpResponseMessage deleteResponse = await _httpCommand.DeleteFile(user.AvatarLocalPath);

                        if (!deleteResponse.IsSuccessStatusCode)
                        {
                            var error = await deleteResponse.Content.ReadAsStringAsync();
                            Console.WriteLine($"---> Ошибка удаления файла: {error}");
                        }
                    }

                    FilesApiResponse fileResult = await uploadResponse.Content.ReadFromJsonAsync<FilesApiResponse>();

                    user.AvatarLocalPath = fileResult.LocalFilePath;
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Ошибка сети: {ex.Message}");
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

            _uow.Users.Update(user);
            await _uow.SaveAsync();

            return Ok(user.Adapt<Models.Dto.UserDTO>());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            User user = _uow.Users.Get(x => x.Id == id);

            if (user == null) return NotFound();

            if (user.AvatarLocalPath != null)
            {
                try
                {
                    Console.WriteLine("--> Запрос к FilesAPI");
                    using HttpResponseMessage response = await _httpCommand.DeleteFile(user.AvatarLocalPath);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"---> Ошибка удаления файла: {error}");
                    }
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

            _uow.Users.Delete(user);

            await _uow.SaveAsync();

            return Ok(user.Adapt<Models.Dto.UserDTO>());
        }
    }
}
