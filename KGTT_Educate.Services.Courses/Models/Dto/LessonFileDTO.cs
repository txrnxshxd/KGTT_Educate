namespace KGTT_Educate.Services.Courses.Models.Dto
{
    public class LessonFileDTO
    {
        public int Id { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string LocalFilePath {  get; set; } = string.Empty;
        public string FullFilePath { get; set; } = string.Empty;
        public bool IsMedia { get; set; }
        public bool IsPinned { get; set; }
    }
}
