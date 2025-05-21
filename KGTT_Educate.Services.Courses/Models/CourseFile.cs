using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class CourseFile
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public string LocalFilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FullFilePath { get; set; } = string.Empty;
        public bool IsMedia { get; set; }
        public bool IsPinned { get; set; }
    }
}
