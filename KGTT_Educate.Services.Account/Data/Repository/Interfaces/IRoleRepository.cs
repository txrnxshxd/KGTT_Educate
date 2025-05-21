using KGTT_Educate.Services.Account.Models;

namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        void Update(Role role);
    }
}
