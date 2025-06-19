using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using KGTT_Educate.Services.Courses.SyncDataServices.Http;
using KGTT_Educate.Services.Courses.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        // СДЕЛАТЬ УДАЛЕНИЕ ФАЙЛОВ АСИНХРОННО

        private readonly IUnitOfWork _uow;
        private readonly ICommandDataClient _httpCommand;

        public LessonsController(IUnitOfWork uow, ICommandDataClient httpCommand)
        {
            _uow = uow;
            _httpCommand = httpCommand;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetAll()
        {
            IEnumerable<Lesson> lessons = await _uow.Lessons.GetAllAsync();

            if (lessons == null || lessons.Count() <= 0) return NotFound();

            return Ok(lessons);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Lesson>> GetById(int id)
        {
            if (id <= 0) return NotFound();

            Lesson lesson = await _uow.Lessons.GetByIdAsync(id);

            return Ok(lesson);
        }

        [HttpGet("Course/{courseId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetByCourseId(int courseId)
        {
            if (courseId <= 0) return BadRequest();

            Course course = await _uow.Courses.GetByIdAsync(courseId);

            if (course == null) return NotFound();

            IEnumerable<Lesson> lessons = await _uow.Lessons.GetByCourseIdAsync(courseId);

            if (lessons == null || lessons.Count() == 0) return NotFound();

            return Ok(lessons);
        }

        [HttpGet("Files/{lessonId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LessonFile>>> GetLessonFiles(int lessonId)
        {
            IEnumerable<LessonFile> files = await _uow.LessonFiles.GetByLessonIdAsync(lessonId);

            if (files == null || files.Count() == 0) return NotFound(new { Message = "Не найдено" });

            return Ok(files);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<Lesson>> Create([FromBody] Lesson lesson)
        {
            if (lesson == null) return BadRequest();

            Lesson lastEl = await _uow.Lessons.GetLastAsync();

            lesson.Id = lastEl == null ? 1 : lastEl.Id + 1;

            Course course = await _uow.Courses.GetByIdAsync(lesson.CourseId);

            if (course == null) return NotFound(new { Message = $"Курс с Id {lesson.CourseId} не существует" });

            await _uow.Lessons.CreateAsync(lesson);

            return Ok(new { message = $"Урок {lesson.Name} успешно создан!" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult> Update(int id, Lesson lesson)
        {
            if (id != lesson.Id) return BadRequest();

            try
            {
                await _uow.Lessons.UpdateAsync(id, lesson);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            IEnumerable<LessonFile> lessonFiles = await _uow.LessonFiles.GetByLessonIdAsync(id);

            if (lessonFiles != null && lessonFiles.Count() > 0)
            {
                foreach (LessonFile lessonFile in lessonFiles)
                {
                    try
                    {
                        using HttpResponseMessage response = await _httpCommand.DeleteFile(lessonFile.LocalFilePath);

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

                }

            }
            await _uow.LessonFiles.DeleteByLessonIdAsync(id);

            await _uow.Lessons.DeleteAsync(id);

            return Ok(new { message = $"Урок удален" });
        }

        [HttpPost("Files/{lessonId}")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult> UploadFile(int lessonId, IFormFile file, bool isPinned = false)
        {
            Lesson lesson = await _uow.Lessons.GetByIdAsync(lessonId);

            if (lesson == null) return NotFound();

            IEnumerable<LessonFile> lessonFiles = await _uow.LessonFiles.GetByLessonIdAsync(lessonId);

            if (lessonFiles.Where(x => x.IsPinned == isPinned).Count() >= 7)
            {
                return BadRequest(new { message = $"Вы не можете загрузить больше 7 {(isPinned ? "Закрепленных файлов" : "Медиафайлов")} к курсу" });
            }

            try
            {
                using HttpResponseMessage response = await _httpCommand.SendFile(file, "Lessons");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Ошибка загрузки файла: {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<FilesApiResponse>();

                LessonFile lastFile = await _uow.LessonFiles.GetLastAsync();

                LessonFile lessonFile = new LessonFile
                {
                    Id = lastFile == null ? 1 : lastFile.Id + 1,
                    LessonId = lessonId,
                    OriginalName = file.FileName,
                    FileName = result.FileName,
                    LocalFilePath = result.LocalFilePath,
                    IsPinned = isPinned
                };

                await _uow.LessonFiles.CreateAsync(lessonFile);

                return Ok();
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
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            LessonFile file = await _uow.LessonFiles.GetByIdAsync(fileId);

            if (file == null) return NotFound();

            try
            {
                using HttpResponseMessage response = await _httpCommand.DeleteFile(file.LocalFilePath);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Ошибка удаления файла: {error}");
                }

                await _uow.LessonFiles.DeleteAsync(fileId);
                return Ok();
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
    }
}
