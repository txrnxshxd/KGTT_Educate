using Google.Protobuf.WellKnownTypes;
using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using KGTT_Educate.Services.Account.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Authentication;
using System.Security.Claims;

namespace KGTT_Educate.Services.Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUoW _uow;
        private readonly JwtService _jwtService;

        public AccountService(IUoW uow, JwtService jwtService)
        {
            _uow = uow;
            _jwtService = jwtService;
        }

        public async Task<Models.Dto.UserDTO> RegisterAsync(User user)
        {

            var passwordHash = new PasswordHasher<User>().HashPassword(user, user.Password);
            user.Password = passwordHash;

            await _uow.Users.AddAsync(user);
            await _uow.SaveAsync();

            return user.Adapt<Models.Dto.UserDTO>();
        }

        public async Task<Models.Dto.UserJwtDTO> LoginAsync(string username, string password)
        {
            // Поиск пользователя
            User user = await _uow.Users.GetByUserNameAsync(username);
            if (user == null)
            {
                throw new AuthenticationException("Неверный логин или пароль");
            }

            // Верификация пароля
            var passwordVerifier = new PasswordHasher<User>();
            var result = passwordVerifier.VerifyHashedPassword(user, user.Password, password);

            switch (result)
            {
                case PasswordVerificationResult.Failed:
                    throw new AuthenticationException("Неверный логин или пароль");

                case PasswordVerificationResult.SuccessRehashNeeded:
                    user.Password = passwordVerifier.HashPassword(user, password);
                    _uow.Users.Update(user);
                    await _uow.SaveAsync();
                    break;
            }

            // Получение ролей пользователя
            var roles = await _uow.UserRole.GetManyAsync(x => x.User.Login == username, includeProperties: "User,Role");
            var userGroups = await _uow.UserGroup.GetManyAsync(ug => ug.UserId == user.Id, includeProperties: "User,Group");

            // Формирование claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Email, user.Email),
            };

            if (!string.IsNullOrEmpty(user.Telegram))
            {
                claims.Add(new Claim("telegram", user.Telegram));
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                claims.Add(new Claim("phoneNumber", user.PhoneNumber));
            }

            // Добавление ролей пользователя в claims
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
                }
            }

            // Генерация токенов
            var tokenPair = await _jwtService.GenerateTokenPairAsync(user.Id.ToString(), claims);

            return new UserJwtDTO
            {
                Id = user.Id,
                Login = user.Login,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Telegram = user.Telegram,
                Groups = userGroups.Select(ug => ug.Group.Name).ToList(),
                AccessToken = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshToken,
                Roles = roles.Select(x => x.Role.Name).ToList(),
                AccessTokenExpiration = _jwtService.GetAccessTokenExpiration()
            };
        }

        public async Task<TokenPair> RefreshTokenAsync(string refreshToken)
        {
            // Валидация refresh токена
            var validationResult = await _jwtService.ValidateRefreshToken(refreshToken);

            if (!validationResult.IsValid)
            {
                throw new SecurityTokenException(
                    validationResult.IsExpired ? "Refresh token expired" : "Invalid refresh token");
            }

            // Получение пользователя
            var userId = validationResult.UserId;
            var user = await _uow.Users.GetAsync(x => x.Id == Guid.Parse(userId));

            if (user == null)
                throw new SecurityTokenException("User not found");

            // Получение claims пользователя
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Email, user.Email),
            };

            // Генерация новой пары токенов
            var newTokenPair = await _jwtService.GenerateTokenPairAsync(userId, claims);

            // Инвалидация старого refresh токена
            await _jwtService.RevokeRefreshToken(refreshToken);

            return newTokenPair;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            await _jwtService.RevokeRefreshToken(refreshToken);
        }

        public async Task RevokeAllRefreshTokensForUserAsync(string userId)
        {
            await _jwtService.RevokeAllRefreshTokensForUser(userId);
        }
    }
}
