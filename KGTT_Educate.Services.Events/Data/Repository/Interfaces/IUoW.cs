namespace KGTT_Educate.Services.Events.Data.Repository.Interfaces
{
    public interface IUoW
    {
        IEventsRepository Events { get; }
        IEventGroupRepository EventGroup { get; }

        void Save();
    }
}
