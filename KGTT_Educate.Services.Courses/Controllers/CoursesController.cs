using KGTT_Educate.Services.Courses.Data.Repository.Interfaces;
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
        private readonly ICourseRepository _courseRepository;

        public CoursesController(ICourseRepository repo, ICourseFilesRepository files)
        {
            _courseRepository = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetAll()
        {
            // ПОЛУЧАЕМ ВСЕ КУРСЫ
            List<Course> courses = await _courseRepository.GetAllAsync();

            if (courses == null || courses.Count == 0) return NotFound();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetById(int id)
        {
            if (id <= 0) return NotFound();
            
            Course course = await _courseRepository.GetByIdAsync(id);

            if (course == null) return NotFound();

            return Ok(course);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<Course>> GetByGroupId(int groupId)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<Course>> Create([FromBody] Course course)
        {
            if (course == null) return BadRequest();

            Course lastEl = await _courseRepository.GetLastAsync();

            course.Id = lastEl == null ? 1 : lastEl.Id + 1;

            await _courseRepository.CreateAsync(course);

            return Ok(new { message = $"Курс {course.Name} успешно создан!" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Course course)
        {
            if (id != course.Id) return BadRequest();

            try
            {
                await _courseRepository.UpdateAsync(id, course);
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

            await _courseRepository.DeleteAsync(id);

            return Ok(new { message = "Курс удален" });
        }
    }
}
