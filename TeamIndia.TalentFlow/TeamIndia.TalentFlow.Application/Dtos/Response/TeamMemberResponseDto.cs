namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class TeamMemberResponseDto
    {
        public Guid TeamMemberId { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public string TeamRole { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
