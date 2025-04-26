using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data
{
    public class LessonMediaService : ILessonMediaService
    {
        private readonly IMongoCollection<LessonMedia> _collection;
        private readonly IFileService _mediaService;

        public LessonMediaService(IMongoDatabase database, IFileService mediaService)
        {
            _collection = database.GetCollection<LessonMedia>("LessonMedia");
            _mediaService = mediaService;
        }

        public async Task<List<LessonMedia>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<LessonMedia> GetByIdAsync(int? id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<LessonMedia>> GetByLessonIdAsync(int? lessonId)
        {
            return await _collection.Find(x => x.LessonId == lessonId).ToListAsync();
        }

        public async Task CreateAsync(LessonMedia media)
        {
            await _collection.InsertOneAsync(media);
        }

        public async Task DeleteAsync(int? id)
        {
            var media = await GetByIdAsync(id);
            if (media != null)
            {
                await _mediaService.DeleteFileAsync(media.MediaPath);
                await _collection.DeleteOneAsync(x => x.Id == id);
            }
        }

        public async Task<LessonMedia> GetLastAsync()
        {
            return await _collection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
