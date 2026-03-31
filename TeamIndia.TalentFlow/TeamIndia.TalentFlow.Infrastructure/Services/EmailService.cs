using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TeamIndia.TalentFlow.Application.ApplicationSettings;
using TeamIndia.TalentFlow.Application.Interfaces;

namespace TeamIndia.TalentFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;

        _smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            UseDefaultCredentials = false,
            EnableSsl = true,
        };

        _smtpClient.Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Pass);
        _fromEmail = _smtpSettings.FromEmail;
        _fromName = _smtpSettings.FromName;
    }

    public void SendConfirmationEmail(string email, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        try
        {
            _smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            // Log the error or handle as needed
            Console.WriteLine($"Error sending confirmation email: {ex.Message}");
        }
    }

    public async Task SendTemplateEmailAsync(string email, string subject, string templateName, IDictionary<string, string> placeholders)
    {
        // templates are stored under Resources/Emails/<templateName>.html
        var basePath = AppContext.BaseDirectory;
        var templatesRoot = Path.Combine(basePath, "Resources", "Emails");
        var filePath = Path.Combine(templatesRoot, templateName + ".html");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Email template not found", filePath);
        }

        var html = await File.ReadAllTextAsync(filePath);
        foreach (var kv in placeholders)
        {
            html = html.Replace("{" + kv.Key + "}", kv.Value);
        }

        await Task.Run(() => SendConfirmationEmail(email, subject, html));
    }
}
