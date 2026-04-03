using System;
using System.Collections.Generic;

namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class CourseResponseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<ModuleResponseDto> Modules { get; set; } = new();
    }
}
