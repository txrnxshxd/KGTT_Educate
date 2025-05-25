namespace KGTT_Educate.Services.FilesAPI.Data.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string section);
        Task DeleteFileAsync(string filePath);
    }
}
