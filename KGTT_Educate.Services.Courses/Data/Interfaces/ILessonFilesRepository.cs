using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface ILessonFilesRepository : IMongoRepository<LessonFile>
    {
        public Task<IEnumerable<LessonFile>> GetByLessonIdAsync(int lessonId);
    }
}
