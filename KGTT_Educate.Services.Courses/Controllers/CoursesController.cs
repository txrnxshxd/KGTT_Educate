using KGTT_Educate.Services.Courses.Data.Interfaces.Services;
using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Data.Services;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using KGTT_Educate.Services.Courses.Utils;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Reflection.Emit;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        //private readonly ICourseRepository _courseRepository;
        //private readonly ICourseFilesRepository _courseFilesRepository;
        private readonly IUnitOfWork _uow;
        private readonly IFileService _fileService;

        public CoursesController(IUnitOfWork uow, IFileService fileService)
        {
            //_courseRepository = repo;
            //_courseFilesRepository = files;
            _uow = uow;
            _fileService = fileService;
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

        [HttpGet("Group/{groupId}")]
        public async Task<ActionResult<Course>> GetByGroupId(int groupId)
        {
            return Ok();
        }

        [HttpGet("Files/Download/{fileId}")]
        public async Task<ActionResult> DownloadFile(int fileId)
        {
            if (fileId <= 0 ) return BadRequest();

            CourseFile file = await _uow.CourseFiles.GetByIdAsync(fileId);

            // ПОЛНЫЙ ПУТЬ
            // FULL PATH
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.LocalFilePath);

            try
            {
                // ПРОБУЕМ СКАЧАТЬ ФАЙЛ
                // TRY TO DOWNLOAD FILE
                await _fileService.DownloadFileAsync(fullPath, HttpContext.Response);
                return new EmptyResult();
            }
            catch (FileNotFoundException)
            {
                // ЕСЛИ НЕ НАШЛИ, КИДАЕМ NF
                // IF FILE WASN'T FOUND, THROW NOT FOUND
                return NotFound();
            }
        }

        [HttpGet("Files/{courseId}")]
        public async Task<ActionResult> GetFilesByCourseId(int courseId)
        {
            IEnumerable<CourseFile> files = await _uow.CourseFiles.GetByCourseIdAsync(courseId);

            if (files == null || files.Count() == 0) return NotFound(new { Message = "Не найдено" });

            return Ok(files.Adapt<IEnumerable<CourseFileDTO>>());
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
                    // ПОЛУЧАЕМ РАСШИРЕНИЕ ПРЕДОСТАВЛЕННОГО ФАЙЛА
                    // GET PROVIDED FILE EXTENSION
                    string fileExt = Path.GetExtension(courseRequest.FormFile.FileName).ToLowerInvariant();

                    if (!AllowedFileExtensions.mediaExtensions.Contains(fileExt))
                        return BadRequest(new { Message = $"Вы не можете загрузить превью с расширением {fileExt}" });

                    string filePath = await _fileService.UploadMediaAsync(courseRequest.FormFile, false);

                    // ОТНОСИТЕЛЬНЫЙ ПУТЬ
                    // RELATIVE PATH
                    var wwwrootPath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), filePath);

                    course.PreviewPhotoPath = filePath;
                    course.LocalPreviewPhotoPath = wwwrootPath;
                }
                catch (Exception ex)
                {
                    //return StatusCode(500, "Ошибка загрузки файла");
                    return StatusCode(500, ex.Message);
                }
            }

            await _uow.Courses.CreateAsync(course);

            return Ok(new { message = $"Курс {course.Name} успешно создан!" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Course course)
        {
            if (id != course.Id) return BadRequest();

            try
            {
                await _uow.Courses.UpdateAsync(id, course);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0 ) return BadRequest();

            IEnumerable<CourseFile> courseFiles = await _uow.CourseFiles.GetByCourseIdAsync(id);

            if (courseFiles != null && courseFiles.Count() > 0)
            {
                foreach (CourseFile courseFile in courseFiles)
                {

                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", courseFile.LocalFilePath);

                    await _fileService.DeleteFileAsync(fullPath);
                }

            }

            IEnumerable<LessonFile> lessonFiles = await _uow.LessonFiles.GetByCourseIdAsync(id);

            if (lessonFiles != null && lessonFiles.Count() > 0)
            {
                foreach (LessonFile lessonFile in lessonFiles)
                {

                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lessonFile.LocalFilePath);

                    await _fileService.DeleteFileAsync(fullPath);
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

            if (course == null) return NotFound();

            try
            {
                // ПОЛУЧАЕМ РАСШИРЕНИЕ ПРЕДОСТАВЛЕННОГО ФАЙЛА
                // GET PROVIDED FILE EXTENSION
                string fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!AllowedFileExtensions.fileExtensions.Contains(fileExt) && !AllowedFileExtensions.mediaExtensions.Contains(fileExt))
                    return BadRequest(new { Message = $"Вы не можете загрузить файл с расширением {fileExt}" });

                bool isMedia = AllowedFileExtensions.mediaExtensions.Contains(fileExt);

                string filePath = await _fileService.UploadFileAsync(file, false, isMedia);

                string fileName = Path.GetFileName(filePath);

                // ОТНОСИТЕЛЬНЫЙ ПУТЬ
                // RELATIVE PATH
                var wwwrootPath = Path.GetRelativePath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), filePath);

                CourseFile lastFile = await _uow.CourseFiles.GetLastAsync();

                CourseFile courseFile = new CourseFile
                {
                    Id = lastFile == null ? 1 : lastFile.Id + 1,
                    CourseId = courseId,
                    OriginalName = file.FileName,
                    FileName = fileName,
                    FullFilePath = filePath,
                    LocalFilePath = wwwrootPath,
                    IsMedia = isMedia,
                    Course = course,
                    IsPinned = isMedia ? isPinned : true // Медиафайлы могут быть и на UI, и как прикрепленный файл, остальные будут помечены как прикрепленный файл
                };

                await _uow.CourseFiles.CreateAsync(courseFile);

                return Ok(new { 
                    WwwrootPath = wwwrootPath, 
                    FileName = fileName, 
                    IsMedia = isMedia, 
                    FilePath = filePath, 
                    OriginalName = courseFile.OriginalName, 
                    IsPinned = courseFile.IsPinned
                });
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "Ошибка загрузки файла");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Files/{fileId}")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            CourseFile file = await _uow.CourseFiles.GetByIdAsync(fileId);

            if (file == null) return NotFound();

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.LocalFilePath);

            await _fileService.DeleteFileAsync(fullPath);

            await _uow.CourseFiles.DeleteAsync(fileId);

            return Ok(new { FilePath = fullPath, FileName = file.FileName });
        }
    }
}
