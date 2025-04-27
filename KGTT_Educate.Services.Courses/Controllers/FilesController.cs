using KGTT_Educate.Services.Courses.Data;
using KGTT_Educate.Services.Courses.Data.Repository.Interfaces;
using KGTT_Educate.Services.Courses.Data.Services.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Common;
using System.Reflection.Emit;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ICourseFilesRepository _courseFilesRepository;
        private readonly ICourseRepository _courseRepository;

        public FilesController(IFileService fileService, ICourseFilesRepository files, ICourseRepository repo)
        {
            _fileService = fileService;
            _courseFilesRepository = files;
            _courseRepository = repo;
        }

        [HttpGet("download/{id}")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            CourseFile courseFile = await _courseFilesRepository.GetByIdAsync(id);
            // ПОЛНЫЙ ПУТЬ
            // FULL PATH
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", courseFile.FilePath);

            try
            {
                // ПРОБУЕМ СКАЧАТЬ ФАЙЛ
                // TRY TO DOWNLOAD FILE
                await _fileService.DownloadFileAsync(fullPath, HttpContext.Response);
                Console.WriteLine(fullPath);
                return new EmptyResult();
            }
            catch (FileNotFoundException)
            {
                // ЕСЛИ НЕ НАШЛИ, КИДАЕМ NF
                // IF FILE WASN'T FOUND, THROW NOT FOUND
                return NotFound();
            }
        }

        [HttpPost("upload/{courseId}")]
        public async Task<ActionResult> UploadFile(IFormFile file, int courseId)
        {
            if (courseId == 0) return NotFound();

            if (file == null || file.Length == 0) return BadRequest();

            // ПРОВЕРЯЕМ, ЕСТЬ ЛИ КУРС, К КОТОРОМУ ХОТИМ ПРИКРЕПИТЬ ФАЙЛ
            // CHECK FOR EXISTING COURSE
            Course course = await _courseRepository.GetByIdAsync(courseId);

            if (course == null) return NotFound();

            try
            {

                // РАЗРЕШЕННЫЕ РАСШИРЕНИЯ ФАЙЛА
                // ALLOWED FILE EXTENSIONS
                string[] fileExtensions = { ".txt", ".docx", ".pdf", ".xls", ".doc", ".zip", ".rar", ".7z" };
                string[] mediaExtensions = { ".jpg", ".jpeg", ".mp4", ".png", ".avi", ".webm" };

                // ПОЛУЧАЕМ РАСШИРЕНИЕ ПРЕДОСТАВЛЕННОГО ФАЙЛА
                // GET PROVIDED FILE EXTENSION
                string fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!fileExtensions.Contains(fileExt) || !mediaExtensions.Contains(fileExt)) return BadRequest(new { Message = $"Вы не можете загрузить файл с расширением {fileExt}" });

                string filePath = await _fileService.SaveFileAsync(file);

                string fileName = Path.GetFileName(filePath);

                // ОТНОСИТЕЛЬНЫЙ ПУТЬ
                // RELATIVE PATH
                var wwwrootPath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), filePath);

                CourseFile courseFile = new()
                {
                    CourseId = course.Id,
                    Course = course,
                    FilePath = fileName
                };

                // ПОЛУЧАЕМ ПОСЛЕДНИЙ ФАЙЛ ИЗ CourseMedia
                // GET LAST FILE FROM CourseMedia
                CourseFile lastEl = await _courseFilesRepository.GetLastAsync();

                // ЕСЛИ НЕ НАШЛИ, ПРИСВАИВАЕМ ID 1, ЕСЛИ НАШЛИ, ПРИСВАИВАЕМ ID ПОСЛЕДНЕГО ЭЛЕМЕНТА + 1
                // IF FILE WASN'T FOUND, ASSIGN ID 1, IF FOUND, ASSIGN LAST ELEMENT ID + 1
                courseFile.Id = lastEl == null ? 1 : lastEl.Id + 1;

                // СОЗДАЕМ ЗАПИСЬ В БАЗЕ
                // CREATE DATABASE ENTRY
                await _courseFilesRepository.CreateAsync(courseFile);

                return Ok(new { CourseId = courseId, FilePath = wwwrootPath });
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Ошибка загрузки файла");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0) return NotFound();

            CourseFile courseMedia = await _courseFilesRepository.GetByIdAsync(id);
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media", courseMedia.FilePath);

            await _fileService.DeleteFileAsync(fullPath);
            await _courseFilesRepository.DeleteAsync(id);

            return Ok(new { Message = $"Файл успешно удален" });
        }
    }
}
