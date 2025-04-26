using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data
{
    public class LessonFileService : ILessonFileService
    {
        private readonly IMongoCollection<LessonFile> _collection;
        private readonly IFileService _fileService;

        public LessonFileService(IMongoDatabase db, IFileService fileService)
        {
            _collection = db.GetCollection<LessonFile>("LessonFile");
            _fileService = fileService;
        }

        public async Task<List<LessonFile>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<LessonFile> GetByIdAsync(int? id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(LessonFile courseFile)
        {
            await _collection.InsertOneAsync(courseFile);
        }

        public async Task UpdateAsync(int? id, LessonFile lessonFile)
        {
            await _collection.ReplaceOneAsync(x => x.Id == id, lessonFile);
        }

        public async Task<LessonFile> GetLastAsync()
        {
            return await _collection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<List<LessonFile>> GetByLessonIdAsync(int? lessonId)
        {
            return await _collection.Find(x => x.LessonId == lessonId).ToListAsync();
        }

        public async Task DeleteAsync(int? id)
        {
            var media = await GetByIdAsync(id);
            if (media != null)
            {
                await _fileService.DeleteFileAsync(media.FilePath);
                await _collection.DeleteOneAsync(x => x.Id == id);
            }
        }
    }
}
