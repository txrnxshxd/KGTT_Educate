using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Events.Data.Repository
{
    public class EventGroupRepository : Repository<EventGroup>, IEventGroupRepository
    {
        private AppDbContext _context;
        public EventGroupRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<EventGroup> GetMany(Expression<Func<EventGroup, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<EventGroup> query = dbSet;

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

        public void Update(EventGroup EventUser)
        {
            _context.EventGroup.Update(EventUser);
        }
    }
}
