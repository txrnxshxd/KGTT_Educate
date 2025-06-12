using KGTT_Educate.Services.Account.Models;

namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task AddAsync(User user);
        Task<User> GetByUserNameAsync(string userName);
        void Update(User user);
    }
}
