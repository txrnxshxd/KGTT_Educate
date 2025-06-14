using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using KGTT_Educate.Services.Account.Models.RequestResponseModels.Request;
using KGTT_Educate.Services.Account.Models.RequestResponseModels.Response;
using KGTT_Educate.Services.Account.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Security.Authentication;
using System.Security.Claims;

namespace KGTT_Educate.Services.Account.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IOptions<JwtSettings> _configuration;

        public AuthController(IAccountService accountService, IOptions<JwtSettings> configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Models.Dto.UserDTO>> Register([FromForm] UserRequest request)
        {
            try
            {
                User user = new User
                {
                    Login = request.Login,
                    Password = request.Password, // Хешируется в сервисе
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Telegram = request.Telegram,
                    LastName = request.LastName,
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _accountService.RegisterAsync(user);
                return Ok(result);
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

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserJwtDTO>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _accountService.LoginAsync(request.Login, request.Password);
                SetRefreshTokenCookie(result.RefreshToken);

                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("Не найден Refresh Token");

            try
            {
                var tokenPair = await _accountService.RefreshTokenAsync(refreshToken);

                // Обновляем refresh token в cookie
                SetRefreshTokenCookie(tokenPair.RefreshToken);

                return Ok(new RefreshTokenResponse
                {
                    AccessToken = tokenPair.AccessToken,
                    AccessTokenExpiration = tokenPair.AccessTokenExpiration
                });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("Logout")]
        [Authorize(Policy = "Authenticated")]
        public async Task<IActionResult> Logout()
        {
            string refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    await _accountService.RevokeRefreshTokenAsync(refreshToken);
                }
                catch
                {
                    // Игнорируем ошибки при инвалидации
                }
            }

            // Очистка cookie
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Успешный выход" });
        }

        [HttpPost("RevokeAll")]
        [Authorize]
        public async Task<IActionResult> RevokeAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("Несуществующий пользователь");

            await _accountService.RevokeAllRefreshTokensForUserAsync(userId);

            // Очистка cookie
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Все сессии отозваны" });
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return;

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Только HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_configuration.Value.RefreshTokenExpirationDays),
                Path = "/"
            });
        }
    }
}
