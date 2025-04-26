using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data
{
    public class CourseFileService : ICourseFileService
    {
        private readonly IMongoCollection<CourseFile> _courseCollection;
        private readonly IFileService _fileService;

        public CourseFileService(IMongoDatabase database, IFileService fileService)
        {
            _courseCollection = database.GetCollection<CourseFile>("CourseFile");
            _fileService = fileService;
        }

        public async Task<List<CourseFile>> GetAllAsync()
        {
            return await _courseCollection.Find(_ => true).ToListAsync();
        }

        public async Task<CourseFile> GetByIdAsync(int? id)
        {
            return await _courseCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CourseFile courseFile)
        {
            await _courseCollection.InsertOneAsync(courseFile);
        }

        public async Task UpdateAsync(int? id, CourseFile courseFile)
        {
            await _courseCollection.ReplaceOneAsync(x => x.Id == id, courseFile);
        }

        public async Task<CourseFile> GetLastAsync()
        {
            return await _courseCollection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<List<CourseFile>> GetByCourseIdAsync(int? courseId)
        {
            return await _courseCollection.Find(x => x.CourseId == courseId).ToListAsync();
        }

        public async Task DeleteAsync(int? id)
        {
            var media = await GetByIdAsync(id);
            if (media != null)
            {
                await _fileService.DeleteFileAsync(media.FilePath);
                await _courseCollection.DeleteOneAsync(x => x.Id == id);
            }
        }
    }
}
