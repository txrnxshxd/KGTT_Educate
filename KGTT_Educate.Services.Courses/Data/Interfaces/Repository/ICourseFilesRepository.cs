﻿using KGTT_Educate.Services.Courses.Models;

namespace KGTT_Educate.Services.Courses.Data.Interfaces.Repository
{
    public interface ICourseFilesRepository : IMongoRepository<CourseFile>
    {
        public Task<IEnumerable<CourseFile>> GetByCourseIdAsync(int courseId);
        public Task DeleteByCourseIdAsync(int courseId);
    }
}
