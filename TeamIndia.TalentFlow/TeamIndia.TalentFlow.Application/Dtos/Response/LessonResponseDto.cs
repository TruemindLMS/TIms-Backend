using System;

namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class LessonResponseDto
    {
        public Guid LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public Guid? ModuleId { get; set; }
    }
}
