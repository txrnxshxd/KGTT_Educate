using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class CourseFilesRepository : MongoRepository<CourseFile>, ICourseFilesRepository
    {
        public CourseFilesRepository(IMongoDatabase db) : base(db)
        {
        }

        public async Task<IEnumerable<CourseFile>> GetByCourseIdAsync(int courseId)
        {
            return await _collection.Find(Builders<CourseFile>.Filter.Eq(x => x.CourseId, courseId)).ToListAsync();
        }

        public async Task DeleteByCourseIdAsync(int courseId)
        {
            await _collection.DeleteManyAsync(x => x.CourseId == courseId);
        }
    }
}
