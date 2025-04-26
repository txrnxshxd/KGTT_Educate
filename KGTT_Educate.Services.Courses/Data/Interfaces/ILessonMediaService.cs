using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IReadOnlyLessonMediaService
    {
        public Task<List<LessonMedia>> GetAllAsync();
        public Task<LessonMedia> GetByIdAsync(int? id);
        public Task<List<LessonMedia>> GetByLessonIdAsync(int? lessonId);
        public Task<LessonMedia> GetLastAsync();
    }
    
    public interface ILessonMediaCommandService
    {
        public Task CreateAsync(LessonMedia media);
        public Task DeleteAsync(int? id);
    }
    
    public interface ILessonMediaService : IReadOnlyLessonMediaService, ILessonMediaCommandService { }
}
