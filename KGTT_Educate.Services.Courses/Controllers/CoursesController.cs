using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using Mapster;
using Microsoft.AspNetCore.Http;
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

        public CoursesController(IUnitOfWork uow)
        {
            //_courseRepository = repo;
            //_courseFilesRepository = files;
            _uow = uow;
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

        [HttpPost]
        public async Task<ActionResult<Course>> Create([FromBody] Course course)
        {
            if (course == null) return BadRequest();

            Course lastEl = await _uow.Courses.GetLastAsync();

            course.Id = lastEl == null ? 1 : lastEl.Id + 1;

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

            if (courseFiles != null && courseFiles.Count() <= 0)
            {
                foreach (CourseFile courseFile in courseFiles)
                {
                    await _uow.CourseFiles.DeleteAsync(courseFile.Id);
                    RedirectToAction($"Delete/{courseFile.FilePath}", "FilesController");
                }
            }

            await _uow.Courses.DeleteAsync(id);

            return Ok(new { message = "Курс удален" });
        }
    }
}
