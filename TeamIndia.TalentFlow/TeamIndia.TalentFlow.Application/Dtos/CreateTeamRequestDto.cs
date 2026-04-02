namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class CreateTeamRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
