using KGTT_Educate.Services.Account.Data.Repository.Interfaces;
using KGTT_Educate.Services.Account.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Account.Data.Repository
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        private AppDbContext _context;

        public UserRoleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(UserRole userRole)
        {
            _context.UserRole.Update(userRole);
        }

        public IEnumerable<UserRole> GetMany(Expression<Func<UserRole, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<UserRole> query = dbSet;

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

        public async Task<IEnumerable<UserRole>> GetManyAsync(Expression<Func<UserRole, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<UserRole> query = dbSet;

            query = query.Where(predicate);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.ToListAsync();
        }
    }
}
