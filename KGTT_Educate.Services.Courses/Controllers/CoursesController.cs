using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using KGTT_Educate.Services.Courses.SyncDataServices.Grpc;
using KGTT_Educate.Services.Courses.SyncDataServices.Http;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ICommandDataClient _httpCommand;
        private readonly IConfiguration _configuration;

        public CoursesController(IUnitOfWork uow, ICommandDataClient httpCommand, IConfiguration configuration)
        {
            _uow = uow;
            _httpCommand = httpCommand;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<Course>>> GetAll()
        {
            IEnumerable<Course> courses = await _uow.Courses.GetAllAsync();

            if (courses == null || courses.Count() <= 0) return NotFound();

            return Ok(courses);
        }

        [HttpGet("GetAllWithGroups")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<CourseGroup>>> GetAllWithGroups()
        {
            var grpcClient = new GrpcAccountClient(_configuration);

            // ПОЛУЧАЕМ ВСЕ КУРСЫ
            IEnumerable<CourseGroup> courses = await _uow.CourseGroup.GetAllAsync();

            // Собираем УНИКАЛЬНЫЕ GroupId
            var uniqueGroupIds = courses
                .Select(c => c.GroupId)
                .Distinct()
                .ToList();

            // Пакетный запрос групп через gRPC
            var groupRequests = uniqueGroupIds.Select(id => grpcClient.GetGroupAsync(id));

            // Параллельное выполнение всех запросов
            var groupResponses = await Task.WhenAll(groupRequests);

            // Создаем словарь для быстрого поиска
            var groupsDict = groupResponses
                .Where(r => r != null)
                .ToDictionary(g => g.Id, g => g);

            foreach (var course in courses)
            {
                if (groupsDict.TryGetValue(course.GroupId, out var group))
                {
                    course.GroupDTO = group;
                }
                else
                {
                    Console.WriteLine($"Группа {course.GroupId} не найдена для курса {course.Id}");
                }
            }

            if (courses == null || courses.Count() <= 0) return NotFound();

            return Ok(courses);
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "Authenticated")]
        public async Task<ActionResult<Course>> GetById(int id)
        {
            if (id <= 0) return NotFound();

            Course course = await _uow.Courses.GetByIdAsync(id);

            if (course == null) return NotFound();

            return Ok(course);
        }

        
        [HttpGet("Group/{groupId}")]
        [Authorize(Policy = "Authenticated")]
        public async Task<ActionResult<Course>> GetByGroupId(Guid groupId)
        {
            var grpcClient = new GrpcAccountClient(_configuration);

            GroupDTO group = await grpcClient.GetGroupAsync(groupId);

            if (group == null) return NotFound("Группа не найдена");

            IEnumerable<CourseGroup> courseGroup = await _uow.CourseGroup.GetByGroupId(groupId);

            if (courseGroup == null) return NotFound($"Не найдено ни одного курса для группы {group.Name}");

            foreach (var item in courseGroup)
            {
                item.GroupDTO = group;
            }

            return Ok(courseGroup);
        }

        [HttpPost("Group")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<ActionResult> AddGroupToCourse(int courseId, Guid groupId)
        {
            Course course = await _uow.Courses.GetByIdAsync(courseId);

            if (course == null) return NotFound(new { message = "Курс не найден" });

            var grpcClient = new GrpcAccountClient(_configuration);

            GroupDTO group = await grpcClient.GetGroupAsync(groupId);

            if (group == null) return NotFound(new { message = "Группа не найдена" });

            CourseGroup last = await _uow.CourseGroup.GetLastAsync();

            CourseGroup courseGroup = new()
            {
                Id = last == null ? 1 : last.Id + 1,
                GroupId = groupId,
                CourseId = courseId,
                Course = course,
                GroupDTO = group
            };
            await _uow.CourseGroup.CreateAsync(courseGroup);

            return Ok(courseGroup);
        }

        [HttpGet("Files/{courseId}")]
        [Authorize(Policy = "Authenticated")]
        public async Task<ActionResult> GetFilesByCourseId(int courseId)
        {
            IEnumerable<CourseFile> files = await _uow.CourseFiles.GetByCourseIdAsync(courseId);

            if (files == null || files.Count() == 0) return NotFound(new { Message = "Не найдено" });

            return Ok(files);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrTeacher")]
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
                    using HttpResponseMessage response = await _httpCommand.SendFile(courseRequest.FormFile, "Courses");

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
        [Authorize(Policy = "AdminOrTeacher")]
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
                    using HttpResponseMessage response = await _httpCommand.SendFile(courseRequest.FormFile, "Courses");

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
                    using HttpResponseMessage response = await _httpCommand.DeleteFile(course.LocalPreviewPhotoPath);

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
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            IEnumerable<CourseFile> courseFiles = await _uow.CourseFiles.GetByCourseIdAsync(id);

            if (courseFiles != null && courseFiles.Count() > 0)
            {
                foreach (CourseFile courseFile in courseFiles)
                {

                    using HttpResponseMessage response = await _httpCommand.DeleteFile(courseFile.LocalFilePath);

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
                    using HttpResponseMessage response = await _httpCommand.DeleteFile(lessonFile.LocalFilePath);

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

        //[HttpGet("Files/Get/{fileId}")]
        //public async Task<ActionResult> GetFile(int fileId)
        //{
        //    if (fileId <= 0) return BadRequest();

        //    CourseFile file = await _uow.CourseFiles.GetByIdAsync(fileId);

        //    try
        //    {
        //        var (fileStream, contentType) = await _httpRead.GetFile(file.LocalFilePath);
        //        return File(fileStream, contentType);
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        return StatusCode((int)ex.StatusCode!, $"Ошибка сети: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        [HttpPost("Files/{courseId}")]
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<ActionResult> UploadFile(int courseId, IFormFile file, bool isPinned = false)
        {
            Course course = await _uow.Courses.GetByIdAsync(courseId);

            if (course == null) return NotFound();

            IEnumerable<CourseFile> courseFiles = await _uow.CourseFiles.GetByCourseIdAsync(course.Id);

            if (courseFiles.Where(x => x.IsPinned == isPinned).Count() >= 5)
            {
                return BadRequest(new { message = $"Вы не можете загрузить больше 5 {(isPinned ? "Закрепленных файлов" : "Медиафайлов")} к курсу"});
            }

            CourseFile lastFile = await _uow.CourseFiles.GetLastAsync();

            try
            {
                using HttpResponseMessage response = await _httpCommand.SendFile(file, "Courses");

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
        [Authorize(Policy = "AdminOrTeacher")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            CourseFile file = await _uow.CourseFiles.GetByIdAsync(fileId);

            if (file == null) return NotFound();

            try
            {
                using HttpResponseMessage response = await _httpCommand.DeleteFile(file.LocalFilePath);

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
