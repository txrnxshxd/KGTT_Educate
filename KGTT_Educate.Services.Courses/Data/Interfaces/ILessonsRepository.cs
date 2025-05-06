using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface ILessonsRepository : IMongoRepository<Lesson>
    {
        public Task UpdateAsync(int id, Lesson course);
        public Task<IEnumerable<Lesson>> GetByCourseId(int courseId);
    }
}
