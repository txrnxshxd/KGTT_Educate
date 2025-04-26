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
        private readonly ICourseService _courses;
        private readonly ICourseMediaService _courseMedia;
        private readonly ICourseFileService _courseFile;

        public CoursesController(ICourseService courses, ICourseMediaService courseMedia, ICourseFileService courseFile)
        {
            _courses = courses;
            _courseMedia = courseMedia;
            _courseFile = courseFile;
        }

        [HttpGet]
        public async Task<ActionResult<List<Course>>> GetAll()
        {
            // ПОЛУЧАЕМ ВСЕ КУРСЫ
            List<Course> courses = await _courses.GetAllAsync();

            if (courses == null || courses.Count == 0) return NotFound();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetById(int? id)
        {
            if (id == null || id <= 0) return NotFound();
            
            Course course = await _courses.GetByIdAsync(id);

            return Ok(course);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<Course>> GetByGroupId(int? groupId)
        {
            return Ok();
        }

        [HttpGet("media/{courseId}")]
        public async Task<ActionResult<List<CourseMedia>>> GetCourseMedia(int? courseId)
        {
            if (courseId == null || courseId <= 0) return NotFound();

            List<CourseMedia> courseMediaList = await _courseMedia.GetByCourseIdAsync(courseId);

            if (courseMediaList == null || courseMediaList.Count <= 0) return NotFound();

            List<CourseMediaDTO> courseMediaDTO = courseMediaList.Adapt<List<CourseMediaDTO>>();

            return Ok(courseMediaDTO);
        }

        [HttpGet("files/{courseId}")]
        public async Task<ActionResult<List<CourseFile>>> GetCourseFiles(int? courseId)
        {
            if (courseId == null || courseId <= 0) return NotFound();

            List<CourseFile> courseFilesList = await _courseFile.GetByCourseIdAsync(courseId);

            if (courseFilesList == null || courseFilesList.Count <= 0) return NotFound();

            List<CourseFileDTO> courseFilesDTO = courseFilesList.Adapt<List<CourseFileDTO>>();

            return Ok(courseFilesDTO);
        }

        [HttpPost]
        public async Task<ActionResult<Course>> Create([FromBody] Course course)
        {
            if (course == null) return BadRequest();

            Course lastEl = await _courses.GetLastAsync();

            course.Id = lastEl == null ? 1 : lastEl.Id + 1;

            await _courses.CreateAsync(course);

            return Ok(new { message = $"Курс {course.Name} успешно создан!" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int? id, Course course)
        {
            if (id != course.Id) return BadRequest();

            try
            {
                await _courses.UpdateAsync(id, course);
            }
            catch (DbUpdateConcurrencyException)
            {

                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            await _courses.DeleteByIdAsync(id);

            return Ok(new { message = "Курс удален" });
        }
    }
}
