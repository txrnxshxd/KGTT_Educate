using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Interfaces.Repository
{
    public interface ICoursesRepository : IMongoRepository<Course>
    {
        public Task UpdateAsync(int id, Course course);
    }
}
