using KGTT_Educate.Services.Events.Models;

namespace KGTT_Educate.Services.Events.Data.Repository.Interfaces
{
    public interface IEventGroupRepository : IRepository<EventGroup>
    {
        void Update(EventGroup EventGroup);
    }
}
