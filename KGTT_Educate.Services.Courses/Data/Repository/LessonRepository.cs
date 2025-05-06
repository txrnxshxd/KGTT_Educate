using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class LessonRepository : MongoRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(IMongoDatabase db) : base(db)
        {

        }

        public async Task UpdateAsync(int id, Lesson lesson)
        {
            await _collection.ReplaceOneAsync(x => x.Id == id, lesson);
        }

        public async Task<IEnumerable<Lesson>> GetByCourseId(int courseId)
        {
            return await _collection.Find(Builders<Lesson>.Filter.Eq(x => x.CourseId, courseId)).ToListAsync();
        }
    }
}
