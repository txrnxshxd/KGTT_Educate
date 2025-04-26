using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IReadOnlyCourseMediaService
    {
        public Task<List<CourseMedia>> GetAllAsync();
        public Task<CourseMedia> GetByIdAsync(int? id);
        public Task<List<CourseMedia>> GetByCourseIdAsync(int? courseId);
        public Task<CourseMedia> GetLastAsync();
    }

    public interface ICourseMediaCommandService
    {
        public Task CreateAsync(CourseMedia media);
        public Task DeleteAsync(int? id);
    }

    public interface ICourseMediaService : IReadOnlyCourseMediaService, ICourseMediaCommandService { }
}
