using MongoDB.Bson.Serialization.Attributes;

namespace KGTT_Educate.Services.Courses.Models.Dto
{
    public class CourseGroupDTO
    {
        public int Id { get; set; }
        public Course Course { get; set; }
        public GroupDTO Group { get; set; }
    }
}
