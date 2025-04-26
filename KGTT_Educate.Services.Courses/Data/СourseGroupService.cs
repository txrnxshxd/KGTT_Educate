using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data
{
    public class СourseGroupService : ICourseGroupService
    {
        private readonly IMongoCollection<CourseGroup> _collection;

        public СourseGroupService(IMongoDatabase db)
        {
            _collection = db.GetCollection<CourseGroup>("CourseGroup");
        }
        public async Task CreateAsync(CourseGroup courseGroup)
        {
            await _collection.InsertOneAsync(courseGroup);
        }

        public async Task DeleteAsync(int? id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<CourseGroup>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<CourseGroup>> GetByGroupIdAsync(int? groupId)
        {
            return await _collection.Find(x => x.GroupId == groupId).ToListAsync();
        }

        public async Task<CourseGroup> GetByIdAsync(int? id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<CourseGroup> GetLastAsync()
        {
            return await _collection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
