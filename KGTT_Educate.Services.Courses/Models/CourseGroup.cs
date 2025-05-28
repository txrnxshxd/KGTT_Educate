using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class CourseGroup
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid GroupId { get; set; }
    }
}
