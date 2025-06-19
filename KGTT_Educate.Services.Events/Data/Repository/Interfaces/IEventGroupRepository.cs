using KGTT_Educate.Services.Events.Models;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Events.Data.Repository.Interfaces
{
    public interface IEventGroupRepository : IRepository<EventGroup>
    {
        void Update(EventGroup EventUser);
        IEnumerable<EventGroup> GetMany(Expression<Func<EventGroup, bool>> predicate, string? includeProperties = null);
    }
}
