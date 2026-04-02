namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class AddTeamMemberRequestDto
    {
        public Guid UserId { get; set; }
        public string TeamRole { get; set; } = string.Empty;
    }
}
