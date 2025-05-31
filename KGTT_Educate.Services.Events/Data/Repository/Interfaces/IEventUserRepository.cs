using KGTT_Educate.Services.Events.Models;
using System.Linq.Expressions;

namespace KGTT_Educate.Services.Events.Data.Repository.Interfaces
{
    public interface IEventUserRepository : IRepository<EventUser>
    {
        void Update(EventUser EventUser);
        IEnumerable<EventUser> GetMany(Expression<Func<EventUser, bool>> predicate, string? includeProperties = null);
    }
}
