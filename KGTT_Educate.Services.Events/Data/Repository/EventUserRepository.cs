using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;

namespace KGTT_Educate.Services.Events.Data.Repository
{
    public class EventUserRepository : Repository<EventUser>, IEventUserRepository
    {
        private AppDbContext _context;
        public EventUserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(EventUser EventUser)
        {
            _context.EventUser.Update(EventUser);
        }
    }
}
