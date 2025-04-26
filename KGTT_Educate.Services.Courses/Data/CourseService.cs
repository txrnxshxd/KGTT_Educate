using KGTT_Educate.Services.Courses.Data.Interfaces;
using KGTT_Educate.Services.Courses.Models;
using KGTT_Educate.Services.Courses.Utils;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KGTT_Educate.Services.Courses.Data
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly ICourseMediaService _courseMediaService;
        private readonly ICourseFileService _courseFileService;
        private readonly IFileService _fileService;
        private readonly IMediaService _mediaService;

        public CourseService(IMongoDatabase database, ICourseMediaService courseRoMediaService, ICourseFileService courseFileService, IFileService fileService, IMediaService mediaService)
        {
            _courseCollection = database.GetCollection<Course>("Courses");
            _courseMediaService = courseRoMediaService;
            _courseFileService = courseFileService;
            _fileService = fileService;
            _mediaService = mediaService;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _courseCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int? id)
        {
            return await _courseCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Course course)
        {
            await _courseCollection.InsertOneAsync(course);
        }

        public async Task UpdateAsync(int? id, Course course)
        {
            await _courseCollection.ReplaceOneAsync(x => x.Id == id, course);
        }

        public async Task DeleteByIdAsync(int? id)
        {
            Course course = await GetByIdAsync(id);
            if (course != null)
            {
                List<CourseMedia> media = await _courseMediaService.GetByCourseIdAsync(course.Id);
                List<CourseFile> files = await _courseFileService.GetByCourseIdAsync(course.Id);

                if (media != null)
                {
                    foreach (var mediaItem in media)
                    {
                        await _courseMediaService.DeleteAsync(mediaItem.Id);
                        await _mediaService.DeleteFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media", mediaItem.MediaPath));
                    }
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        await _courseFileService.DeleteAsync(file.Id);
                        await _fileService.DeleteFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", file.FilePath));
                    }
                }

                await _courseCollection.DeleteOneAsync(x => x.Id == id);
            }
        }

        public async Task<Course> GetLastAsync()
        {
            return await _courseCollection.Find(_ => true).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
