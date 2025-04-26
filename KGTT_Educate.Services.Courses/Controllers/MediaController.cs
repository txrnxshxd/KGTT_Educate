using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly ICourseMediaService _courseMediaService;
        private readonly ICourseService _courseService;

        public MediaController(ICourseMediaService courseMediaService, ICourseService courseService, IMediaService mediaService)
        {
            _courseMediaService = courseMediaService;
            _courseService = courseService;
            _mediaService = mediaService;
        }

        [HttpPost("upload/{courseId}")]
        public async Task<ActionResult> UploadMedia(IFormFile file, int? courseId)
        {
            if (file == null || file.Length == 0) return BadRequest();

            // ПРОВЕРЯЕМ, ЕСТЬ ЛИ КУРС, К КОТОРОМУ ХОТИМ ПРИКРЕПИТЬ ФАЙЛ
            // CHECK FOR EXISTING COURSE
            Course course = await _courseService.GetByIdAsync(courseId);

            if (course == null) return NotFound();

            try
            {
                // РАЗРЕШЕННЫЕ РАСШИРЕНИЯ МЕДИА ФАЙЛА
                // ALLOWED MEDIA FILE EXTENSIONS
                string[] allowedMediaExtensions = { ".jpg", ".jpeg", ".mp4", ".png", ".avi", ".webm" };

                // ПОЛУЧАЕМ РАСШИРЕНИЕМ ПРЕДОСТАВЛЕННОГО ФАЙЛА
                // GET PROVIDED FILE EXTENSION
                string fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (allowedMediaExtensions.Contains(fileExt))
                {
                    string filePath = await _mediaService.SaveFileAsync(file);

                    string fileName = Path.GetFileName(filePath);

                    // ОТНОСИТЕЛЬНЫЙ ПУТЬ
                    // RELATIVE PATH
                    var wwwrootPath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), filePath);

                    CourseMedia courseMedia = new()
                    {
                        CourseId = course.Id,
                        Course = course,
                        MediaPath = fileName
                    };

                    // ПОЛУЧАЕМ ПОСЛЕДНИЙ ФАЙЛ ИЗ CurseMedia
                    // GET LAST FILE FROM CurseMedia
                    CourseMedia lastEl = await _courseMediaService.GetLastAsync();

                    // ЕСЛИ НЕ НАШЛИ, ПРИСВАИВАЕМ ID 1, ЕСЛИ НАШЛИ, ПРИСВАИВАЕМ ID ПОСЛЕДНЕГО ЭЛЕМЕНТА + 1
                    // IF FILE WASN'T FOUND, ASSIGN ID 1, IF FOUND, ASSIGN LAST ELEMENT ID + 1
                    courseMedia.Id = lastEl == null ? 1 : lastEl.Id + 1;

                    // СОЗДАЕМ ЗАПИСЬ В БАЗЕ
                    // CREATE DATABASE ENTRY
                    await _courseMediaService.CreateAsync(courseMedia);

                    return Ok(new { CourseId = courseId, FilePath = wwwrootPath });
                }

                return BadRequest(new { Message = $"Вы не можете загрузить файл с расширением {fileExt}" });
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Ошибка загрузки файла");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null || id <= 0) return NotFound();

            CourseMedia courseMedia = await _courseMediaService.GetByIdAsync(id);
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media", courseMedia.MediaPath);

            await _mediaService.DeleteFileAsync(fullPath);
            await _courseMediaService.DeleteAsync(id);

            return Ok(new { Message = $"Файл успешно удален" });
        }
    }
}
