using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace KGTT_Educate.Services.Courses.Controllers
{
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessons;
        private readonly ILessonMediaService _lessonMedia;
        private readonly ILessonFileService _lessonFile;

        public LessonsController(ILessonService lessons, ILessonMediaService media, ILessonFileService file)
        {
            _lessons = lessons;
            _lessonMedia = media;
            _lessonFile = file;
        }

        [HttpGet]
        public async Task<ActionResult<List<Lesson>>> GetAll()
        {
            List<Lesson> lessons = await _lessons.GetAllAsync();

            if (lessons == null || lessons.Count <= 0) return NotFound();

            return Ok(lessons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetById(int? id)
        {
            if (id == null || id <= 0) return NotFound();

            Lesson lesson = await _lessons.GetByIdAsync(id);

            return Ok(lesson);
        }

        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<Course>> GetByCourseId(int? courseId)
        {
            return Ok();
        }

        [HttpGet("Media/{lessonId}")]
        public async Task<ActionResult<List<LessonMedia>>> GetLessonMedia(int? lessonId)
        {
            if (lessonId == null || lessonId <= 0) return NotFound();

            List<LessonMedia> lessonMediaList = await _lessonMedia.GetByLessonIdAsync(lessonId);

            if (lessonMediaList == null || lessonMediaList.Count <= 0) return NotFound();

            List<LessonMediaDTO> courseMediaDTO = lessonMediaList.Adapt<List<LessonMediaDTO>>();

            return Ok(courseMediaDTO);
        }

        [HttpGet("Files/{lessonId}")]
        public async Task<ActionResult<List<CourseFile>>> GetLessonFiles(int? lessonId)
        {
            if (lessonId == null || lessonId <= 0) return NotFound();

            List<LessonFile> courseFilesList = await _lessonFile.GetByLessonIdAsync(lessonId);

            if (courseFilesList == null || courseFilesList.Count <= 0) return NotFound();

            List<LessonFileDTO> courseFilesDTO = courseFilesList.Adapt<List<LessonFileDTO>>();

            return Ok(courseFilesDTO);
        }
    }
}
