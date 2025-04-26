using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class LessonMedia
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        [ForeignKey(nameof(LessonId))]
        public Lesson Lesson { get; set; }

        public string MediaPath { get; set; } = string.Empty;
    }
}
