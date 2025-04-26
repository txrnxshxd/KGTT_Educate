using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IReadOnlyCourseGroupService
    {
        public Task<List<CourseGroup>> GetAllAsync();
        public Task<CourseGroup> GetByIdAsync(int? id);
        public Task<List<CourseGroup>> GetByGroupIdAsync(int? courseId);
        public Task<CourseGroup> GetLastAsync();
    }

    public interface ICommandCourseGroupService
    {
        public Task CreateAsync(CourseGroup courseGroup);
        public Task DeleteAsync(int? id);
    }

    public interface ICourseGroupService : IReadOnlyCourseGroupService, ICommandCourseGroupService { }
}
