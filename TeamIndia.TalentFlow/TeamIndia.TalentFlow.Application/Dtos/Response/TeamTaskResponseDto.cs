namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class TeamTaskResponseDto
    {
        public Guid TeamTaskId { get; set; }
        public Guid TeamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public DateTime? DueDateUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
