using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IReadOnlyCourseFileService
    {
        public Task<List<CourseFile>> GetAllAsync();
        public Task<CourseFile> GetByIdAsync(int? id);
        public Task<List<CourseFile>> GetByCourseIdAsync(int? courseId);
        public Task<CourseFile> GetLastAsync();
    }

    public interface ICourseFileCommandService
    {
        public Task CreateAsync(CourseFile media);
        public Task DeleteAsync(int? id);
    }

    public interface ICourseFileService : IReadOnlyCourseFileService, ICourseFileCommandService { }
}
