using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Account.Data.Repository
{
    public class UserGroupRepository : Repository<UserGroup>, IUserGroupRepository
    {
        private AppDbContext _context;

        public UserGroupRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<UserGroup> GetMany(Expression<Func<UserGroup, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<UserGroup> query = dbSet;

            query = query.Where(predicate);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return query.ToList();
        }

        public void Update(UserGroup userGroup)
        {
            _context.UserGroup.Update(userGroup);
        }
    }
}
