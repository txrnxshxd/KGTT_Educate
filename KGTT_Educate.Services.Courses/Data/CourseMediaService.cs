using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data
{
    public class CourseMediaService : ICourseMediaService
    {
        private readonly IMongoCollection<CourseMedia> _courseMediaCollection;
        private readonly IFileService _mediaService;

        public CourseMediaService(IMongoDatabase database, IFileService mediaService)
        {
            _courseMediaCollection = database.GetCollection<CourseMedia>("CourseMedia");
            _mediaService = mediaService;
        }

        public async Task<List<CourseMedia>> GetAllAsync()
        {
            return await _courseMediaCollection.Find(_ => true).ToListAsync();
        }

        public async Task<CourseMedia> GetByIdAsync(int? id)
        {
            return await _courseMediaCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<CourseMedia>> GetByCourseIdAsync(int? courseId)
        {
            return await _courseMediaCollection.Find(x => x.CourseId == courseId).ToListAsync();
        }

        public async Task CreateAsync(CourseMedia media)
        {
            await _courseMediaCollection.InsertOneAsync(media);
        }

        public async Task DeleteAsync(int? id)
        {
            var media = await GetByIdAsync(id);
            if (media != null)
            {
                await _mediaService.DeleteFileAsync(media.MediaPath);
                await _courseMediaCollection.DeleteOneAsync(x => x.Id == id);
            }
        }

        public async Task<CourseMedia> GetLastAsync()
        {
            return await _courseMediaCollection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
