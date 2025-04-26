using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IReadOnlyCourseService
    {
        public Task<List<Course>> GetAllAsync();
        public Task<Course> GetByIdAsync(int? id);
        public Task<Course> GetLastAsync();
    }

    public interface ICourseCommandService
    {
        public Task CreateAsync(Course course);
        public Task UpdateAsync(int? id, Course course);
        public Task DeleteByIdAsync(int? id);
    }

    public interface ICourseService : IReadOnlyCourseService, ICourseCommandService { }
}
