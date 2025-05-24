using KGTT_Educate.Services.Account.Models;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        IEnumerable<UserRole> GetMany(Expression<Func<UserRole, bool>> predicate, string? includeProperties = null);
        void Update(UserRole userRole);
    }
}
