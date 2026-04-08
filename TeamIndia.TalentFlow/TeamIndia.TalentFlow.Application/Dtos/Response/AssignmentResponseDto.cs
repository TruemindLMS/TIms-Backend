namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class AssignmentResponseDto
    {
        public Guid AssignmentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDateUtc { get; set; }
    }
}
