using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Models.Dto;

namespace KGTT_Educate.Services.Courses.Data.Interfaces.Repository
{
    public interface ILessonsRepository : IMongoRepository<Lesson>
    {
        public Task UpdateAsync(int id, Lesson lesson);
        public Task<IEnumerable<Lesson>> GetByCourseIdAsync(int courseId);
        public Task DeleteByCourseIdAsync(int courseId);
    }
}
