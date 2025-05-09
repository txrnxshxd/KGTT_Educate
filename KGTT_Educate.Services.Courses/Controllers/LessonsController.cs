using KGTT_Educate.Services.Courses.Data.Interfaces.Services;
using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using KGTT_Educate.Services.Courses.Utils;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        //private readonly ILessonsRepository _lessonsRepository;
        //private readonly ILessonFilesRepository _lessonFile;
        private readonly IUnitOfWork _uow;
        private readonly IFileService _fileService;

        public LessonsController(IUnitOfWork uow, IFileService fileService)
        {
            //_lessonsRepository = lessons;
            //_lessonFile = file;
            _uow = uow;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetAll()
        {
            IEnumerable<Lesson> lessons = await _uow.Lessons.GetAllAsync();

            if (lessons == null || lessons.Count() <= 0) return NotFound();

            return Ok(lessons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetById(int id)
        {
            if (id <= 0) return NotFound();

            Lesson lesson = await _uow.Lessons.GetByIdAsync(id);

            return Ok(lesson);
        }

        [HttpGet("Course/{courseId}")]
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
        public async Task<ActionResult<IEnumerable<LessonFile>>> GetLessonFiles(int lessonId)
        {
            if (lessonId <= 0) return NotFound();

            IEnumerable<LessonFile> courseFilesList = await _uow.LessonFiles.GetByLessonIdAsync(lessonId);

            if (courseFilesList == null || courseFilesList.Count() <= 0) return NotFound();

            List<LessonFileDTO> courseFilesDTO = courseFilesList.Adapt<List<LessonFileDTO>>();

            return Ok(courseFilesDTO);
        }

        [HttpPost]
        public async Task<ActionResult<Lesson>> Create([FromBody] Lesson lesson)
        {
            if (lesson == null) return BadRequest();

            Lesson lastEl = await _uow.Lessons.GetLastAsync();

            lesson.Id = lastEl == null ? 1 : lastEl.Id + 1;

            Course course = await _uow.Courses.GetByIdAsync(lesson.CourseId);

            if (course == null ) return NotFound(new { Message = $"Курс с Id {lesson.CourseId} не существует"});

            await _uow.Lessons.CreateAsync(lesson);

            return Ok(new { message = $"Урок {lesson.Name} успешно создан!" });
        }

        [HttpPut("{id}")]
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
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            IEnumerable<LessonFile> lessonFiles = await _uow.LessonFiles.GetByCourseIdAsync(id);

            if (lessonFiles != null && lessonFiles.Count() > 0)
            {
                foreach (LessonFile lessonFile in lessonFiles)
                {

                    bool isMedia = AllowedFileExtensions.mediaExtensions.Contains(Path.GetExtension(lessonFile.FilePath));

                    string directory = isMedia ? "Media" : "Files";

                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Lessons", directory, lessonFile.FilePath);

                    await _fileService.DeleteFileAsync(fullPath);
                }

            }


            await _uow.Lessons.DeleteByCourseIdAsync(id);

            await _uow.LessonFiles.DeleteByCourseIdAsync(id);

            return Ok(new { message = $"Урок удален" });
        }

        [HttpPost("Files/Upload/{lessonId}")]
        public async Task<ActionResult> UploadFile(int lessonId, IFormFile file)
        {
            Lesson lesson = await _uow.Lessons.GetByIdAsync(lessonId);

            if (lesson == null) return NotFound();

            try
            {
                // ПОЛУЧАЕМ РАСШИРЕНИЕ ПРЕДОСТАВЛЕННОГО ФАЙЛА
                // GET PROVIDED FILE EXTENSION
                string fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!AllowedFileExtensions.fileExtensions.Contains(fileExt) && !AllowedFileExtensions.mediaExtensions.Contains(fileExt))
                    return BadRequest(new { Message = $"Вы не можете загрузить файл с расширением {fileExt}" });

                bool isMedia = AllowedFileExtensions.mediaExtensions.Contains(fileExt);

                string filePath = await _fileService.UploadFileAsync(file, true, isMedia);

                string fileName = Path.GetFileName(filePath);

                // ОТНОСИТЕЛЬНЫЙ ПУТЬ
                // RELATIVE PATH
                var wwwrootPath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Lessons"), filePath);

                LessonFile lastFile = await _uow.LessonFiles.GetLastAsync();

                LessonFile lessonFile = new LessonFile
                {
                    Id = lastFile == null ? 1 : lastFile.Id + 1,
                    LessonId = lessonId,
                    FilePath = fileName,
                    IsMedia = isMedia,
                    Lesson = lesson,
                };

                await _uow.LessonFiles.CreateAsync(lessonFile);

                return Ok(new { WwwrootPath = wwwrootPath, FileName = fileName, IsMedia = isMedia });
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Ошибка загрузки файла");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Files/Delete/{fileId}")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            LessonFile file = await _uow.LessonFiles.GetByIdAsync(fileId);

            if (file == null) return NotFound();

            string directory = file.IsMedia ? "Media" : "Files";

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Lessons", directory, file.FilePath);

            await _fileService.DeleteFileAsync(fullPath);

            await _uow.LessonFiles.DeleteAsync(fileId);

            return Ok(new { FilePath = fullPath, FileName = file.FilePath });
        }
    }
}
