using KGTT_Educate.Services.Events.Models;

namespace KGTT_Educate.Services.Events.Data.Repository.Interfaces
{
    public interface IEventUserRepository : IRepository<EventUser>
    {
        void Update(EventUser EventUser);
    }
}
