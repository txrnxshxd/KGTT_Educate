namespace KGTT_Educate.Services.Courses.Models.Dto
{
    public class CourseRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? FormFile { get; set; }
    }
}
