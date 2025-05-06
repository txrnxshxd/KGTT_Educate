namespace KGTT_Educate.Services.Courses.Data.Interfaces
{
    public interface IUnitOfWork
    {
        ICourseFilesRepository CourseFiles { get; }
        ICoursesRepository Courses { get; }
        ILessonFilesRepository LessonFiles { get; }
        ILessonsRepository Lessons { get; }
    }
}
