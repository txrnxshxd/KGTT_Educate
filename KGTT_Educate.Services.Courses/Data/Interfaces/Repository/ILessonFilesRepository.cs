using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces.Repository
{
    public interface ILessonFilesRepository : IMongoRepository<LessonFile>
    {
        public Task<IEnumerable<LessonFile>> GetByLessonIdAsync(int lessonId);
        public Task<IEnumerable<LessonFile>> GetByCourseIdAsync(int courseId);
        public Task DeleteByLessonIdAsync(int lessonId);
        public Task DeleteByCourseIdAsync(int courseId);
    }
}
