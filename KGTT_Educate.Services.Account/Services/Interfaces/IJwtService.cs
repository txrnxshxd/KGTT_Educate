using KGTT_Educate.Services.Account.Models.Dto;
using System.Security.Claims;

namespace KGTT_Educate.Services.Account.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        Task<string> GenerateAndStoreRefreshToken(string userId);
        DateTime GetAccessTokenExpiration();
        Task<RefreshTokenValidationResult> ValidateRefreshToken(string refreshToken);
        Task RevokeRefreshToken(string refreshToken);
        Task RevokeAllRefreshTokensForUser(string userId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
