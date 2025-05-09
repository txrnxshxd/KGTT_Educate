using KGTT_Educate.Services.Courses.Models.Dto;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class CourseGroup
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
        public int GroupId { get; set; }
    }
}
