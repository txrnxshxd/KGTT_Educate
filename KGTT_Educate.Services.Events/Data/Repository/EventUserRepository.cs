using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Events.Data.Repository
{
    public class EventUserRepository : Repository<EventUser>, IEventUserRepository
    {
        private AppDbContext _context;
        public EventUserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<EventUser> GetMany(Expression<Func<EventUser, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<EventUser> query = dbSet;

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

        public void Update(EventUser EventUser)
        {
            _context.EventUser.Update(EventUser);
        }
    }
}
