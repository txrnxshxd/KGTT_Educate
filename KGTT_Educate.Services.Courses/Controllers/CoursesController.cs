using KGTT_Educate.Services.Courses.Data.Interfaces.Services;
using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

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
        private readonly IHttpClientFactory _httpClient;

        public CoursesController(IUnitOfWork uow, IFileService fileService, IHttpClientFactory httpClient)
        {
            //_courseRepository = repo;
            //_courseFilesRepository = files;
            _uow = uow;
            _fileService = fileService;
            _httpClient = httpClient;
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


        //TODO
        [HttpGet("Files/Download/{fileId}")]
        public async Task<ActionResult> DownloadFile(int fileId)
        {
            if (fileId <= 0) return BadRequest();

            CourseFile file = await _uow.CourseFiles.GetByIdAsync(fileId);

            if (file == null) return NotFound();

            // ПОЛНЫЙ ПУТЬ
            // FULL PATH
            var httpClient = _httpClient.CreateClient();

            using var httpResponse = await httpClient.GetAsync($"http://192.168.0.37:10005/Download/{file.LocalFilePath}");

            //if (httpResponse.IsSuccessStatusCode)
            //{
            //    using var contentStream = await httpResponse.Content.ReadAsStreamAsync();


            //}

            return Ok();
        }

        //TODO
        [HttpGet("Files/{courseId}")]
        public async Task<ActionResult> GetFilesByCourseId(int courseId)
        {
            IEnumerable<CourseFile> files = await _uow.CourseFiles.GetByCourseIdAsync(courseId);

            if (files == null || files.Count() == 0) return NotFound(new { Message = "Не найдено" });

            //return Ok(files.Adapt<IEnumerable<FileDTO>>());
            return Ok();
        }

        //TODO
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
                //var httpClient = _httpClient.CreateClient();
                //using var response = await httpClient.PostAsync($"http://192.168.0.37:10005/Upload", courseRequest.FormFile);

                //course.PreviewPhotoPath = ;
                //course.LocalPreviewPhotoPath = wwwrootPath;
            }

            await _uow.Courses.CreateAsync(course);

            return Ok(new { message = $"Курс {course.Name} успешно создан!" });
        }

        //TODO
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

        //TODO
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

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


        //TODO
        [HttpPost("Files/{courseId}")]
        public async Task<ActionResult> UploadFile(int courseId, IFormFile file, bool isPinned = false)
        {
            Course course = await _uow.Courses.GetByIdAsync(courseId);

            if (course == null) return NotFound();

            //try
            //{

            //CourseFile courseFile = new CourseFile
            //{
            //    Id = lastFile == null ? 1 : lastFile.Id + 1,
            //    CourseId = courseId,
            //    OriginalName = file.FileName,
            //    FileName = fileName,
            //    FullFilePath = filePath,
            //    LocalFilePath = wwwrootPath,
            //    IsMedia = isMedia,
            //    Course = course,
            //    IsPinned = isMedia ? isPinned : true // Медиафайлы могут быть и на UI, и как прикрепленный файл, остальные будут помечены как прикрепленный файл
            //};

            //await _uow.CourseFiles.CreateAsync(courseFile);

            //return Ok(new { 
            //    WwwrootPath = wwwrootPath, 
            //    FileName = fileName, 
            //    IsMedia = isMedia, 
            //    FilePath = filePath, 
            //    OriginalName = courseFile.OriginalName, 
            //    IsPinned = courseFile.IsPinned
            //});
            //}
            //catch (Exception ex)
            //{
            //    //return StatusCode(500, "Ошибка загрузки файла");
            //    return StatusCode(500, ex.Message);
            //}

            return Ok();
        }

        // TODO
        [HttpDelete("Files/{fileId}")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            CourseFile file = await _uow.CourseFiles.GetByIdAsync(fileId);

            if (file == null) return NotFound();


            await _uow.CourseFiles.DeleteAsync(fileId);

            return Ok(new { FileName = file.FileName });
        }
    }
}
