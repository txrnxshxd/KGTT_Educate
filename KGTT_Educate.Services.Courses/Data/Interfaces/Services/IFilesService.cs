using KGTT_Educate.Services.Courses.Utils;

namespace KGTT_Educate.Services.Courses.Data.Interfaces.Services
{
    public interface IFilesService
    {
        Task DownloadFileAsync(string filePath, HttpResponse response);
        Task<string> UploadFileAsync(IFormFile file, bool isLesson, bool isMedia);
        Task<string> UploadMediaAsync(IFormFile file, bool isLesson);
        Task DeleteFileAsync(string filePath);
    }
}
