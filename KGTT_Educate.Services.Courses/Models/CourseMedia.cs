using KGTT_Educate.Services.Courses.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class CourseMedia
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        public string MediaPath { get; set; } = string.Empty;
    }
}
