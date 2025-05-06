using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KGTT_Educate.Services.Courses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonsRepository _lessonsRepository;
        private readonly ILessonFilesRepository _lessonFile;

        public LessonsController(ILessonsRepository lessons, ILessonFilesRepository file)
        {
            _lessonsRepository = lessons;
            _lessonFile = file;
        }

        [HttpGet]
        public async Task<ActionResult<List<Lesson>>> GetAll()
        {
            IEnumerable<Lesson> lessons = await _lessonsRepository.GetAllAsync();

            if (lessons == null || lessons.Count() <= 0) return NotFound();

            return Ok(lessons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetById(int id)
        {
            if (id <= 0) return NotFound();

            Lesson lesson = await _lessonsRepository.GetByIdAsync(id);

            return Ok(lesson);
        }

        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<Course>> GetByCourseId(int? courseId)
        {
            return Ok();
        }

        [HttpGet("Files/{lessonId}")]
        public async Task<ActionResult<IEnumerable<CourseFile>>> GetLessonFiles(int lessonId)
        {
            if (lessonId <= 0) return NotFound();

            IEnumerable<LessonFile> courseFilesList = await _lessonFile.GetByLessonIdAsync(lessonId);

            if (courseFilesList == null || courseFilesList.Count() <= 0) return NotFound();

            List<LessonFileDTO> courseFilesDTO = courseFilesList.Adapt<List<LessonFileDTO>>();

            return Ok(courseFilesDTO);
        }

        [HttpPost]
        public async Task<ActionResult<Course>> Create([FromBody] Lesson lesson)
        {
            if (lesson == null) return BadRequest();

            Lesson lastEl = await _lessonsRepository.GetLastAsync();

            lesson.Id = lastEl == null ? 1 : lastEl.Id + 1;

            await _lessonsRepository.CreateAsync(lesson);

            return Ok(new { message = $"Курс {lesson.Name} успешно создан!" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Lesson lesson)
        {
            if (id != lesson.Id) return BadRequest();

            try
            {
                await _lessonsRepository.UpdateAsync(id, lesson);
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
            if (id <= 0) return NotFound();

            await _lessonsRepository.DeleteAsync(id);

            return Ok(new { message = "Курс удален" });
        }
    }
}
