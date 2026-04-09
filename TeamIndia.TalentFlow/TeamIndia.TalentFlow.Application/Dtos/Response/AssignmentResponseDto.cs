namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    using TeamIndia.TalentFlow.Application.Enums;

    public class AssignmentResponseDto
    {
        public Guid AssignmentId { get; set; }
        public Guid CourseId { get; set; }
        public Guid? LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDateUtc { get; set; }
        public AssignmentStatus Status { get; set; }
    }
}
