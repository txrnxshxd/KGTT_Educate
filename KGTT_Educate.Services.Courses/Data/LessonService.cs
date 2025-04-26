using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data
{
    public class LessonService : ILessonService
    {
        private readonly IMongoCollection<Lesson> _collection;

        public LessonService(IMongoDatabase db)
        {
            _collection = db.GetCollection<Lesson>("Lessons");
        }
        public async Task CreateAsync(Lesson lesson)
        {
            await _collection.InsertOneAsync(lesson);
        }

        public async Task DeleteAsync(int? id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task EditAsync(Lesson lesson, int? id)
        {
            var filter = Builders<Lesson>.Filter.Eq(x => x.Id, id);
            await _collection.ReplaceOneAsync(filter, lesson);
        }

        public async Task<List<Lesson>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<Lesson>> GetByCourseIdAsync(int? courseId)
        {
            return await _collection.Find(x => x.CourseId == courseId).ToListAsync();
        }

        public async Task<Lesson> GetByIdAsync(int? id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Lesson> GetLastAsync()
        {
            return await _collection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
