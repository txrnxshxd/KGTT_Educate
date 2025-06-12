using KGTT_Educate.Services.Account.Models;
using KGTT_Educate.Services.Account.Models.Dto;

namespace KGTT_Educate.Services.Account.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Models.Dto.UserDTO> RegisterAsync(User user);
        Task<UserJwtDTO> LoginAsync(string username, string password);
        Task<TokenPair> RefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAllRefreshTokensForUserAsync(string userId);
    }
}
