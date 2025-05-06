using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class LessonFilesRepository : MongoRepository<LessonFile>, ILessonFilesRepository
    {
        public LessonFilesRepository(IMongoDatabase db) : base(db)
        {
        }

        public async Task<IEnumerable<LessonFile>> GetByLessonIdAsync(int lessonId)
        {
            return await _collection.Find(Builders<LessonFile>.Filter.Eq(x => x.LessonId, lessonId)).ToListAsync();
        }
    }
}
