namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IOtpService
{
    Task<string> GenerateAndStoreOtpAsync(string email, TimeSpan? ttl = null);
    Task<bool> VerifyOtpAsync(string email, string code);
}
