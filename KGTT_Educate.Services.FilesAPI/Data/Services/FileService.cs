using KGTT_Educate.Services.FilesAPI.Data.Interfaces.Services;
using KGTT_Educate.Services.FilesAPI.Utils;

namespace KGTT_Educate.Services.FilesAPI.Data.Services
{
    public class FileService : IFileService
    {
        private IConfiguration _configuration;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string section)
        {
            long allowedFileLength = 50 * 1024 * 1024;

            if (file.Length > allowedFileLength)
            {
                throw new Exception($"Размер файла не может быть более {allowedFileLength / 1024 / 1024} мб");
            }

            // Расширение файла
            string fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Имя файла
            string fileName = $"{Guid.NewGuid().ToString()}{fileExt}";

            // Получаем путь к директории, указанной в appsettings.json
            var directorySection = _configuration.GetSection($"FileStorage:{section}:RootPath");

            if (!directorySection.Exists() || directorySection.Value == null)
            {
                throw new InvalidOperationException($"RootPath для секции {section} не задан.");
            }

            string filesDirectory = AllowedFileExtensions.MediaExtensions.Contains(fileExt) ? Path.Combine(directorySection.Value, "Media") : Path.Combine(directorySection.Value, "Files");

            if (!Directory.Exists(filesDirectory))
            {
                Directory.CreateDirectory(filesDirectory);
            }

            string filePath = Path.Combine(filesDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), filePath));
            }

            return Task.CompletedTask;
        }
    }
}
