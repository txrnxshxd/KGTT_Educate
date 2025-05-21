using KGTT_Educate.Services.Account.Models;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        void Update(UserRole userRole);
        IEnumerable<UserRole> GetMany(Expression<Func<UserRole, bool>> predicate, string? includeProperties = null);
    }
}
