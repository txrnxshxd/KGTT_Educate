using KGTT_Educate.Services.Courses.Data.Repository.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.Repository
{
    public abstract class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase db, string collectionName)
        {
            _collection = db.GetCollection<T>(collectionName);
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync();
        }

        public async Task<T> GetLastAsync()
        {
            return await _collection.Find(_ => true).FirstOrDefaultAsync();
        }
    }
}
