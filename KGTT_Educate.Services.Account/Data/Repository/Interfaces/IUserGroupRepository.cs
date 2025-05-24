using KGTT_Educate.Services.Account.Models;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Account.Data.Repository.Interfaces
{
    public interface IUserGroupRepository : IRepository<UserGroup>
    {
        IEnumerable<UserGroup> GetMany(Expression<Func<UserGroup, bool>> predicate, string? includeProperties = null);
        public void Update(UserGroup userGroup);
        
    }
}
