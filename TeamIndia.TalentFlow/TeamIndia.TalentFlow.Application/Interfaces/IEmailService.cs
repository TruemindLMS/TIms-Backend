namespace TeamIndia.TalentFlow.Application.Interfaces;

public interface IEmailService
{
    //void SendConfirmationEmail(string email, string subject, string body);
    Task SendTemplateEmailAsync(string email, string subject, string templateName, IDictionary<string, string> placeholders);
    Task SendConfirmationEmail(string email, string subject, string body);
    Task SendEmailWithAttachmentAsync(string email, string subject, string htmlBody, string attachmentName, byte[] attachmentBytes);
}