using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IReadOnlyLessonService
    {
        public Task<List<Lesson>> GetAllAsync();
        public Task<Lesson> GetByIdAsync(int? id);
        public Task<List<Lesson>> GetByCourseIdAsync(int? courseId);
        public Task<Lesson> GetLastAsync();
    }

    public interface ILessonCommandService
    {
        public Task CreateAsync(Lesson lesson);
        public Task DeleteAsync(int? id);
        public Task EditAsync(Lesson lesson, int? id);
    }

    public interface ILessonService : IReadOnlyLessonService, ILessonCommandService { }
}
