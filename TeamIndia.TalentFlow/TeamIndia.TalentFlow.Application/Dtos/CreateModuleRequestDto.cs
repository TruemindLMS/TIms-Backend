using System.Collections.Generic;

namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class CreateModuleRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public List<CreateLessonRequestDto>? Lessons { get; set; }
    }
}
