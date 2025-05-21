using KGTT_Educate.Services.Account.Models;

namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        void Update(User user);
    }
}
