namespace KGTT_Educate.Services.Courses.Models.Dto
{
    public class CourseFileDTO
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public bool IsMedia { get; set; }
    }
}
