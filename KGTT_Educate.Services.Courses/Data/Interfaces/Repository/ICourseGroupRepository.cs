using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces.Repository
{
    public interface ICourseGroupRepository : IMongoRepository<CourseGroup>
    {
        public Task<IEnumerable<CourseGroup>> GetByCourseIdAsync(int courseId);
        public Task<IEnumerable<CourseGroup>> GetByGroupId(Guid groupId);
        public Task DeleteByCourseIdAsync(int courseId);
        public Task DeleteCourseGroup(int courseId, Guid groupId);
    }
}
