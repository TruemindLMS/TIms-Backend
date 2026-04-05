namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IEmailService
{
    Task SendConfirmationEmail(string email, string subject, string body);
    Task SendTemplateEmailAsync(string email, string subject, string templateName, IDictionary<string, string> placeholders);
}
