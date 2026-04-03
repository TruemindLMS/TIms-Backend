using System;
using System.Collections.Generic;

namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class ModuleResponseDto
    {
        public Guid ModuleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public List<LessonResponseDto> Lessons { get; set; } = new();
    }
}
