namespace TeamIndia.TalentFlow.Application.Dtos.Response
{
    public class TeamDetailsResponseDto
    {
        public Guid TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public IEnumerable<TeamMemberResponseDto> Members { get; set; } = new List<TeamMemberResponseDto>();
        public IEnumerable<TeamTaskResponseDto> Tasks { get; set; } = new List<TeamTaskResponseDto>();
        public IEnumerable<TeamUpdateResponseDto> Updates { get; set; } = new List<TeamUpdateResponseDto>();

    }
}
