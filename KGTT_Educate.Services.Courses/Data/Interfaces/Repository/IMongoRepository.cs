namespace KGTT_Educate.Services.Courses.Data.Interfaces.Repository
{
    public interface IMongoRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);
        public Task<T> GetLastAsync();
        public Task CreateAsync(T entity);
        public Task DeleteAsync(int id);
    }
}
