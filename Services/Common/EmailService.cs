using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Services.Common;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var emailConfig = _configuration.GetSection("Email");
        var from = emailConfig["From"] ?? message.From;
        var smtpServer = emailConfig["SmtpServer"];
        var port = int.TryParse(emailConfig["Port"], out var p) ? p : 587;
        var user = emailConfig["User"];
        var password = emailConfig["Password"];
        var enableSsl = bool.TryParse(emailConfig["EnableSsl"], out var ssl) ? ssl : true;

        var mail = new MailMessage
        {
            From = new MailAddress(from, message.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsBodyHtml
        };
        mail.To.Add(message.To);
        if (!string.IsNullOrEmpty(message.Cc))
            mail.CC.Add(message.Cc);
        if (!string.IsNullOrEmpty(message.Bcc))
            mail.Bcc.Add(message.Bcc);

        using var smtp = new SmtpClient(smtpServer, port)
        {
            Credentials = new NetworkCredential(user, password),
            EnableSsl = enableSsl
        };
        await smtp.SendMailAsync(mail);
    }
}

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public bool IsBodyHtml { get; set; } = true;
}
