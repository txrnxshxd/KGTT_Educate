using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class CourseFile
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public bool IsMedia;
    }
}
