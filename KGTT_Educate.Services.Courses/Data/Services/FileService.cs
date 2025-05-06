using KGTT_Educate.Services.Courses.Data.Interfaces.Services;

namespace KGTT_Educate.Services.Courses.Data.Services
{
    public class FileService : IFilesService
    {
        private readonly string _coursesStoragePath;
        private readonly string _lessonsStoragePath;

        public FileService(string coursesStoragePath, string lessonsStoragePath)
        {
            _coursesStoragePath = coursesStoragePath;
            _lessonsStoragePath = lessonsStoragePath;
        }

        public async Task<string> UploadFileAsync(IFormFile file, bool isLesson = false, bool isMedia = false)
        {
            string directory = isLesson ? _lessonsStoragePath : _coursesStoragePath;
            
            string fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";

            string filesDirectory = isMedia ? Path.Combine(directory, "Media") : Path.Combine(directory, "Files");

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

        public async Task<string> UploadMediaAsync(IFormFile file, bool isLesson = false)
        {
            return await UploadFileAsync(file, isLesson, true);
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
