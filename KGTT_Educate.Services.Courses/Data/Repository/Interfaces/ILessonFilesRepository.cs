using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Repository.Interfaces
{
    public interface ILessonFilesRepository : IMongoRepository<LessonFile>
    {
        public Task<List<LessonFile>> GetByLessonIdAsync(int lessonId);
    }
}
