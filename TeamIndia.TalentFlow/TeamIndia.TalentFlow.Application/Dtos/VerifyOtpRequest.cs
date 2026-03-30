namespace TeamIndia.TalentFlow.Application.Dtos;

public class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
