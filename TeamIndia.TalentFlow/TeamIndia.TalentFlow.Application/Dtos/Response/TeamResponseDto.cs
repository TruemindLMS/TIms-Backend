namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class TeamResponseDto
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
