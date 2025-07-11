﻿using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Courses.Models
{
    public class LessonFile
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        [ForeignKey(nameof(LessonId))]
        public Lesson? Lesson { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public string LocalFilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
    }
}
