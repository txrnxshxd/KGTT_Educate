using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;

namespace KGTT_Educate.Services.Events.Data.Repository
{
    public class EventsRepository : Repository<Event>, IEventsRepository
    {
        private AppDbContext _context;
        public EventsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Event Event)
        {
            _context.Events.Update(Event);
        }
    }
}
