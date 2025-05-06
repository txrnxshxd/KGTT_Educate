using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Bson;
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

        public async Task<IEnumerable<LessonFile>> GetByCourseIdAsync(int courseId)
        {
            return await _collection.Find(x => x.Lesson.CourseId == courseId).ToListAsync();
        }

        public async Task DeleteByLessonIdAsync(int lessonId)
        {
            await _collection.DeleteManyAsync(x => x.LessonId == lessonId);
        }

        public async Task DeleteByCourseIdAsync(int courseId)
        {
            await _collection.DeleteManyAsync(x => x.Lesson.CourseId == courseId);
        }

    }
}
