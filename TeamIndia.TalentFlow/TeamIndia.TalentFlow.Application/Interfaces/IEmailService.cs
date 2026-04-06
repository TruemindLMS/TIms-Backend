namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IEmailService
{
    //void SendConfirmationEmail(string email, string subject, string body);
    Task SendTemplateEmailAsync(string email, string subject, string templateName, IDictionary<string, string> placeholders);
    Task SendConfirmationEmail(string email, string subject, string body);
}