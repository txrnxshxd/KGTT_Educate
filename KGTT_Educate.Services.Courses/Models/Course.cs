namespace KGTT_Educate.Services.Courses.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PreviewPhotoPath { get; set; }
    }
}
