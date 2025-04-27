using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Repository.Interfaces
{
    public interface ILessonRepository : IMongoRepository<Lesson>
    {
        public Task UpdateAsync(int id, Lesson course);
        public Task<List<Lesson>> GetByCourseId(int courseId);
    }
}
