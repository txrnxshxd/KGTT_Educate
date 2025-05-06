namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IUnitOfWork
    {
        ICourseFilesRepository CourseFiles { get; }
        ICourseRepository Courses { get; }
        ILessonFilesRepository LessonFiles { get; }
        ILessonRepository Lessons { get; }
    }
}
