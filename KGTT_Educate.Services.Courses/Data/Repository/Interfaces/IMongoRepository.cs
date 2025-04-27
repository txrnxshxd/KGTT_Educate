namespace KGTT_Educate.Services.Courses.Data.Repository.Interfaces
{
    public interface IMongoRepository<T> where T : class
    {
        public Task<List<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);
        public Task<T> GetLastAsync();
        public Task CreateAsync(T entity);
        public Task DeleteAsync(int id);
    }
}
