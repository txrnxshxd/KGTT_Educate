using KGTT_Educate.Services.Events.Data.Repository.Interfaces;

namespace KGTT_Educate.Services.Events.Data.Repository
{
    public class UoW : IUoW
    {
        private AppDbContext _context;

        public IEventsRepository Events { get; private set; }

        public IEventGroupRepository EventGroup { get; private set; }

        public IEventUserRepository EventUser { get; private set; }

 

        public UoW(AppDbContext context)
        {
            _context = context;

            Events = new EventsRepository(context);
            EventGroup = new EventGroupRepository(context);
            EventUser = new EventUserRepository(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
