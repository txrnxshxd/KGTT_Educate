using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;
using KGTT_Educate.Services.Courses.Models;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public class CourseGroupRepository : MongoRepository<CourseGroup>, ICourseGroupRepository
    {
        public CourseGroupRepository(IMongoDatabase db) : base(db)
        {
        }

        public async Task DeleteByCourseIdAsync(int courseId)
        {
            await _collection.DeleteManyAsync(x => x.CourseId == courseId);
        }

        public async Task<IEnumerable<CourseGroup>> GetByCourseIdAsync(int courseId)
        {
            return await _collection.Find(Builders<CourseGroup>.Filter.Eq(x => x.CourseId, courseId)).ToListAsync();
        }

        public async Task<IEnumerable<CourseGroup>> GetByGroupId(Guid groupId)
        {
            return await _collection.Find(Builders<CourseGroup>.Filter.Eq(x => x.GroupId, groupId)).ToListAsync();
        }

        public async Task DeleteCourseGroup(int courseId, Guid groupId)
        {
            await _collection.DeleteManyAsync(x => x.GroupId == groupId && x.CourseId == courseId);
        }
    }
}
