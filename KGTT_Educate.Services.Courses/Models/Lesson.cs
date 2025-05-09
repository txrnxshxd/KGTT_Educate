using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }
}
