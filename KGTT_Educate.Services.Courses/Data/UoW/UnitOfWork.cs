using KGTT_Educate.Services.Courses.Data.Interfaces.Repository;
using KGTT_Educate.Services.Courses.Data.Interfaces.UoW;
using KGTT_Educate.Services.Courses.Data.Repository;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        public IMongoDatabase _db;
        public ICourseFilesRepository CourseFiles { get; private set; }

        public ICoursesRepository Courses { get; private set; }

        public ILessonFilesRepository LessonFiles { get; private set; }

        public ILessonsRepository Lessons { get; private set; }

        public UnitOfWork(IMongoDatabase db)
        {
            _db = db;
            CourseFiles = new CourseFilesRepository(db);
            Courses = new CoursesRepository(db);
            LessonFiles = new LessonFilesRepository(db);
            Lessons = new LessonsRepository(db);
        }
    }
}
