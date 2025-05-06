using KGTT_Educate.Services.Courses.Data.Interfaces;

namespace KGTT_Educate.Services.Courses.Data.Services
{
    public class FileService : IFilesService
    {
        private readonly string _defaultStoragePath;
        private readonly string _mediaStoragePath;

        public FileService(string fileStoragePath, string mediaStoragePath)
        {
            _defaultStoragePath = fileStoragePath;
            _mediaStoragePath = mediaStoragePath;
        }

        public async Task<string> UploadFileAsync(IFormFile file, bool isMedia)
        {
            var targetPath = isMedia ? _mediaStoragePath : _defaultStoragePath;

            var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(targetPath, fileName);

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
            //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileInfo.Name}\"");
                response.Headers.Add("Content-Length", fileInfo.Length.ToString());
                response.ContentType = "application/octet-stream";

                await fileStream.CopyToAsync(response.Body);
                await response.Body.FlushAsync();
            }
        }
    }
}
