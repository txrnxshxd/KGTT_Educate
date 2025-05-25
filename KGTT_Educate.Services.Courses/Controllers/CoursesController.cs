using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using KGTT_Educate.Services.Courses.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        // СДЕЛАТЬ УДАЛЕНИЕ ФАЙЛОВ АСИНХРОННО
        private readonly IUnitOfWork _uow;
        private readonly ICommandDataClient _http;

        public CoursesController(IUnitOfWork uow, ICommandDataClient http)
        {
            _uow = uow;
            _http = http;
        }


        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetAll()
        {
            // ПОЛУЧАЕМ ВСЕ КУРСЫ
            IEnumerable<Course> courses = await _uow.Courses.GetAllAsync();

            if (courses == null || courses.Count() <= 0) return NotFound();

            return Ok(courses);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetById(int id)
        {
            if (id <= 0) return NotFound();

            Course course = await _uow.Courses.GetByIdAsync(id);

            if (course == null) return NotFound();

            return Ok(course);
        }


        //TODO
        [HttpGet("Group/{groupId}")]
        public async Task<ActionResult<Course>> GetByGroupId(int groupId)
        {
            return Ok();
        }

        [HttpGet("Files/{courseId}")]
        public async Task<ActionResult> GetFilesByCourseId(int courseId)
        {
            IEnumerable<CourseFile> files = await _uow.CourseFiles.GetByCourseIdAsync(courseId);

            if (files == null || files.Count() == 0) return NotFound(new { Message = "Не найдено" });

            return Ok(files);
        }

        [HttpPost]
        public async Task<ActionResult<CourseRequest>> Create([FromForm] CourseRequest courseRequest)
        {
            if (courseRequest == null) return BadRequest();

            Course lastEl = await _uow.Courses.GetLastAsync();

            Course course = new Course
            {
                Id = lastEl == null ? 1 : lastEl.Id + 1,
                Name = courseRequest.Name,
                Description = courseRequest.Description
            };

            if (courseRequest.FormFile != null)
            {
                try
                {
                    using HttpResponseMessage response = await _http.SendFile(courseRequest.FormFile, "Courses");

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, $"Ошибка загрузки файла: {error}");
                    }

                    var result = await response.Content.ReadFromJsonAsync<FilesApiResponse>();

                    course.LocalPreviewPhotoPath = result.LocalFilePath;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка сети: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Ошибка JSON: {ex.Message}");;
                }
            }

            await _uow.Courses.CreateAsync(course);

            return Ok(new { message = $"Курс {course.Name} успешно создан!" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CourseRequest courseRequest)
        {
            if (courseRequest == null || id == 0) return BadRequest();

            Course course = await _uow.Courses.GetByIdAsync(id);

            if (course == null) return NotFound();

            course.Name = courseRequest.Name;
            course.Description = courseRequest.Description;

            if (courseRequest.FormFile != null)
            {
                try
                {
                    using HttpResponseMessage response = await _http.SendFile(courseRequest.FormFile, "Courses");

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(StatusCode((int)response.StatusCode, $"Ошибка загрузки файла: {error}"));
                    }

                    var result = await response.Content.ReadFromJsonAsync<FilesApiResponse>();

                    course.LocalPreviewPhotoPath = result.LocalFilePath;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка сети: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Ошибка JSON: {ex.Message}"); ;
                }
            }
            else
            {
                if (course.LocalPreviewPhotoPath != null)
                {
                    using HttpResponseMessage response = await _http.DeleteFile(course.LocalPreviewPhotoPath);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(StatusCode((int)response.StatusCode, $"Ошибка удаления файла: {error}"));
                    }
                    else
                    {
                        course.LocalPreviewPhotoPath = null;
                    }

                }
            }

            try
            {
                await _uow.Courses.UpdateAsync(id, course);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest();
            }

            return Ok(course);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            IEnumerable<CourseFile> courseFiles = await _uow.CourseFiles.GetByCourseIdAsync(id);

            if (courseFiles != null && courseFiles.Count() > 0)
            {
                foreach (CourseFile courseFile in courseFiles)
                {

                    using HttpResponseMessage response = await _http.DeleteFile(courseFile.LocalFilePath);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(StatusCode((int)response.StatusCode, $"Ошибка удаления файла: {error}"));
                    }
                }

            }

            IEnumerable<LessonFile> lessonFiles = await _uow.LessonFiles.GetByCourseIdAsync(id);

            if (lessonFiles != null && lessonFiles.Count() > 0)
            {
                foreach (LessonFile lessonFile in lessonFiles)
                {
                    using HttpResponseMessage response = await _http.DeleteFile(lessonFile.LocalFilePath);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(StatusCode((int)response.StatusCode, $"Ошибка удаления файла: {error}"));
                    }
                }
            }


            await _uow.Lessons.DeleteByCourseIdAsync(id);

            await _uow.Courses.DeleteAsync(id);

            await _uow.CourseFiles.DeleteByCourseIdAsync(id);

            await _uow.LessonFiles.DeleteByCourseIdAsync(id);

            return Ok(new { message = "Курс удален" });
        }


        [HttpPost("Files/{courseId}")]
        public async Task<ActionResult> UploadFile(int courseId, IFormFile file, bool isPinned = false)
        {
            Course course = await _uow.Courses.GetByIdAsync(courseId);
            CourseFile lastFile = await _uow.CourseFiles.GetLastAsync();

            if (course == null) return NotFound();

            try
            {
                using HttpResponseMessage response = await _http.SendFile(file, "Courses");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Ошибка загрузки файла: {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<FilesApiResponse>();

                course.LocalPreviewPhotoPath = result.LocalFilePath;

                CourseFile courseFile = new CourseFile
                {
                    Id = lastFile == null ? 1 : lastFile.Id + 1,
                    CourseId = courseId,
                    OriginalName = result.OriginalName,
                    FileName = result.FileName,
                    LocalFilePath = result.LocalFilePath,
                    Course = course,
                    IsPinned = isPinned
                };

                await _uow.CourseFiles.CreateAsync(courseFile);

                return Ok(courseFile);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Ошибка сети: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Ошибка JSON: {ex.Message}");
            }
        }

        [HttpDelete("Files/{fileId}")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            CourseFile file = await _uow.CourseFiles.GetByIdAsync(fileId);

            if (file == null) return NotFound();

            try
            {
                using HttpResponseMessage response = await _http.DeleteFile(file.LocalFilePath);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Ошибка удаления файла: {error}");
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Ошибка сети: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Ошибка JSON: {ex.Message}");
            }

            await _uow.CourseFiles.DeleteAsync(fileId);

            return Ok(file);
        }
    }
}
