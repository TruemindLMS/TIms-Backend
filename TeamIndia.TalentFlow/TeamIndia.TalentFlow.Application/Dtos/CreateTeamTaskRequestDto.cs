namespace TeamIndia.TalentFlow.Application.Dtos
{
    public class CreateTeamTaskRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public DateTime? DueDateUtc { get; set; }
    }
}
