using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class CourseRepository : MongoRepository<Course>, ICourseRepository
    {
        public CourseRepository(IMongoDatabase db) : base(db)
        {
        }

        public async Task UpdateAsync(int id, Course course)
        {
            await _collection.ReplaceOneAsync(x => x.Id == id, course);
        }
    }
}
