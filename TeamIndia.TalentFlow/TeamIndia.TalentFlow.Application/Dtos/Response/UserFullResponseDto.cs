namespace TeamIndia.TalentFlow.Application.Dtos.Response;

public class UserFullResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public IList<string>? Roles { get; set; }
    public ProfileDto? Profile { get; set; }
    public OnboardingDto? Onboarding { get; set; }
}
