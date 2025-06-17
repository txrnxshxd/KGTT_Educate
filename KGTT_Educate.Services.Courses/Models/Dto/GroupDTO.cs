using MongoDB.Bson.Serialization.Attributes;

namespace KGTT_Educate.Services.Courses.Models.Dto
{
    public class GroupDTO
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
