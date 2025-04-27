using KGTT_Educate.Services.Courses.Models;
using Microsoft.EntityFrameworkCore;

namespace KGTT_Educate.Services.Courses.Data.Repository.Interfaces
{
    public interface ICourseFilesRepository : IMongoRepository<CourseFile>
    {
        public Task<List<CourseFile>> GetByCourseIdAsync(int courseId);
    }
}
