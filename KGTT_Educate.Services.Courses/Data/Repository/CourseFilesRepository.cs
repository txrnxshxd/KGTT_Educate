using KGTT_Educate.Services.Courses.Data.Repository.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class CourseFilesRepository : MongoRepository<CourseFile>, ICourseFilesRepository
    {
        public CourseFilesRepository(IMongoDatabase db, string collectionName) : base(db, collectionName)
        {
        }

        public async Task<List<CourseFile>> GetByCourseIdAsync(int courseId)
        {
            return await _collection.Find(Builders<CourseFile>.Filter.Eq(x => x.CourseId, courseId)).ToListAsync();
        }
    }
}
