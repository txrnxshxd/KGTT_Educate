using KGTT_Educate.Services.Courses.Data.Interfaces;

namespace KGTT_Educate.Services.Courses.Data
{
    public class FileService : IFileService
    {
        private readonly string _fileStoragePath;

        public FileService(string fileStoragePath)
        {
            _fileStoragePath = fileStoragePath;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid().ToString()}_{file.FileName}";
            var filePath = Path.Combine(_fileStoragePath, fileName);

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
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        public async Task DownloadFileAsync(string filePath, HttpResponse response)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            var fileInfo = new FileInfo(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileInfo.Name}\"");
            response.Headers.Add("Content-Length", fileInfo.Length.ToString());
            response.ContentType = "application/octet-stream";

            await fileStream.CopyToAsync(response.Body);
            await response.Body.FlushAsync();

            fileStream.Dispose();
        }
    }
}
