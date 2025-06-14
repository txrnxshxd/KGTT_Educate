using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;
using KGTT_Educate.Services.Account.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace KGTT_Educate.Services.Account.Services
{
    public class JwtService
    {
        private readonly JwtSettings _settings;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _redisDb;

        public JwtService(IOptions<JwtSettings> settings, IConnectionMultiplexer redis)
        {
            _settings = settings.Value;
            _redis = redis;
            _redisDb = _redis.GetDatabase();
        }

        // Удалено дублирование claims при генерации токена
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = creds,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Генерация и сохранение refresh-токена
        public async Task<string> GenerateAndStoreRefreshToken(string userId)
        {
            // Генерация криптографически безопасного токена
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            // Ключ для хранения в Redis
            var redisKey = $"refresh_token:{refreshToken}";

            // Данные для сохранения
            var tokenData = new RefreshTokenData
            {
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays)
            };

            // Сериализуем данные в JSON
            var tokenJson = JsonSerializer.Serialize(tokenData);

            // Сохраняем в Redis с установкой времени жизни
            await _redisDb.StringSetAsync(
                redisKey,
                tokenJson,
                TimeSpan.FromDays(_settings.RefreshTokenExpirationDays));

            return refreshToken;
        }

        public DateTime GetAccessTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
        }

        public async Task<RefreshTokenValidationResult> ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return RefreshTokenValidationResult.Invalid;

            var redisKey = $"refresh_token:{refreshToken}";
            var tokenJson = await _redisDb.StringGetAsync(redisKey);

            if (tokenJson.IsNullOrEmpty)
                return RefreshTokenValidationResult.Invalid;

            try
            {
                var tokenData = JsonSerializer.Deserialize<RefreshTokenData>(tokenJson);

                if (tokenData == null)
                    return RefreshTokenValidationResult.Invalid;

                // Проверка срока действия
                if (tokenData.Expires < DateTime.UtcNow)
                {
                    await _redisDb.KeyDeleteAsync(redisKey);
                    return RefreshTokenValidationResult.Expired;
                }

                // Дополнительные проверки (например, device fingerprint)
                // if (tokenData.DeviceFingerprint != GetCurrentDeviceFingerprint())
                //     return RefreshTokenValidationResult.Invalid;

                return new RefreshTokenValidationResult
                {
                    IsValid = true,
                    UserId = tokenData.UserId
                };
            }
            catch
            {
                return RefreshTokenValidationResult.Invalid;
            }
        }

        public async Task<TokenPair> GenerateTokenPairAsync(string userId, IEnumerable<Claim> claims)
        {
            var accessToken = GenerateAccessToken(claims);
            var refreshToken = await GenerateAndStoreRefreshToken(userId);

            return new TokenPair
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = GetAccessTokenExpiration()
            };
        }

        public async Task RevokeRefreshToken(string refreshToken)
        {
            var redisKey = $"refresh_token:{refreshToken}";
            await _redisDb.KeyDeleteAsync(redisKey);
        }

        // Отзыв всех токенов пользователя
        public async Task RevokeAllRefreshTokensForUser(string userId)
        {
            // Шаблон для поиска ключей
            var pattern = $"refresh_token:*";
            var server = _redis.GetServer(_redis.GetEndPoints().First());

            // Ищем все ключи по шаблону
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                var value = await _redisDb.StringGetAsync(key);
                if (!value.IsNullOrEmpty)
                {
                    var tokenData = JsonSerializer.Deserialize<RefreshTokenData>(value);
                    if (tokenData != null && tokenData.UserId == userId)
                    {
                        await _redisDb.KeyDeleteAsync(key);
                    }
                }
            }
        }
    }
}
