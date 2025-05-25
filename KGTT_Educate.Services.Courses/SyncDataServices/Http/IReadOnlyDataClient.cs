namespace KGTT_Educate.Services.Courses.SyncDataServices.Http
{
    public interface IReadOnlyDataClient
    {
        Task<(Stream FileStream, string ContentType)> GetFile(string path);
    }
}
