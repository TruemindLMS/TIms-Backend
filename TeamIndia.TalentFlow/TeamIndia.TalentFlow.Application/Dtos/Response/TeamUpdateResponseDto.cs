namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class TeamUpdateResponseDto
    {
        public Guid TeamUpdateId { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public string? UserFullName { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
