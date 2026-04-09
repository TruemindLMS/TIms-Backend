using TeamIndia.TalentFlow.Domain.Enums;

namespace TeamIndia.TalentFlow.Application.Dtos.Response;

public class OnboardingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public Discipline? Discipline { get; set; }
    public Goal? Goal { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
