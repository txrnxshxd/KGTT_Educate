using KGTT_Educate.Services.Account.Models;

namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IGroupRepository : IRepository<Group>
    {
        void Update(Group group);
    }
}
