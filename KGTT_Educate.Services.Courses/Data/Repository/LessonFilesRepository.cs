using KGTT_Educate.Services.Courses.Data.Repository.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class LessonFilesRepository : MongoRepository<LessonFile>, ILessonFilesRepository
    {
        public LessonFilesRepository(IMongoDatabase db, string collectionName) : base(db, collectionName)
        {
        }

        public async Task<List<LessonFile>> GetByLessonIdAsync(int lessonId)
        {
            return await _collection.Find(Builders<LessonFile>.Filter.Eq(x => x.LessonId, lessonId)).ToListAsync();
        }
    }
}
