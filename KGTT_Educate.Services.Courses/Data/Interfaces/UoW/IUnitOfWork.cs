using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;

namespace KGTT_Educate.Services.Courses.Data.Interfaces.UoW
{
    public interface IUnitOfWork
    {
        ICourseFilesRepository CourseFiles { get; }
        ICoursesRepository Courses { get; }
        ILessonFilesRepository LessonFiles { get; }
        ILessonsRepository Lessons { get; }
        ICourseGroupRepository CourseGroup { get; }
    }
}
