namespace KGTT_Educate.Services.Events.Data.Repository.Interfaces
{
    public interface IUoW
    {
        IEventsRepository Events { get; }
        IEventUserRepository EventUser { get; }

        void Save();
    }
}
