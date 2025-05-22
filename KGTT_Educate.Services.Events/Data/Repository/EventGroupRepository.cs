using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;

namespace KGTT_Educate.Services.Events.Data.Repository
{
    public class EventGroupRepository : Repository<EventGroup>, IEventGroupRepository
    {
        private AppDbContext _context;
        public EventGroupRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(EventGroup EventGroup)
        {
            _context.EventGroup.Update(EventGroup);
        }
    }
}
