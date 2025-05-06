using KGTT_Educate.Services.Courses.Utils;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IFileService
    {
        Task DownloadFileAsync(string filePath, HttpResponse response);
        Task<string> UploadFileAsync(IFormFile file, bool isMedia);
        Task DeleteFileAsync(string filePath);
    }
}
