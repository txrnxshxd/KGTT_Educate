namespace KGTT_Educate.Services.FilesAPI.Data.Interfaces.Services
{
    public interface IFileService
    {
        Task DownloadFileAsync(string filePath, HttpResponse response);
        Task<string> UploadFileAsync(IFormFile file, string section);
        Task DeleteFileAsync(string filePath);
    }
}
