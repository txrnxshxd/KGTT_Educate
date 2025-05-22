using KGTT_Educate.Services.Events.Models;

namespace KGTT_Educate.Services.Events.Data.Repository.Interfaces
{
    public interface IEventsRepository : IRepository<Event>
    {
        void Update(Event Event);
    }
}
