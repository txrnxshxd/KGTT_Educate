using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IReadOnlyLessonFileService
    {
        public Task<List<LessonFile>> GetAllAsync();
        public Task<LessonFile> GetByIdAsync(int? id);
        public Task<List<LessonFile>> GetByLessonIdAsync(int? courseId);
        public Task<LessonFile> GetLastAsync();
    }

    public interface ILessonFileCommandService
    {
        public Task CreateAsync(LessonFile file);
        public Task DeleteAsync(int? id);
    }

    public interface ILessonFileService : IReadOnlyLessonFileService, ILessonFileCommandService { }
}
