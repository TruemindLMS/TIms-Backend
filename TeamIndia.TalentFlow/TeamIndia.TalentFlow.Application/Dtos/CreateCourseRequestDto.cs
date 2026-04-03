using System.Collections.Generic;

namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class CreateCourseRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<CreateModuleRequestDto>? Modules { get; set; }
    }
}
