using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class LessonsRepository : MongoRepository<Lesson>, ILessonsRepository
    {
        public LessonsRepository(IMongoDatabase db) : base(db)
        {

        }

        public async Task UpdateAsync(int id, Lesson lesson)
        {
            await _collection.ReplaceOneAsync(x => x.Id == id, lesson);
        }

        public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(int courseId)
        {
            return await _collection.Find(Builders<Lesson>.Filter.Eq(x => x.CourseId, courseId)).ToListAsync();
        }

        public async Task DeleteByCourseIdAsync(int courseId)
        {
            await _collection.DeleteManyAsync(x => x.CourseId == courseId);
        }
    }
}
