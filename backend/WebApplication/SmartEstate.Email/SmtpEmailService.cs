using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using Contracts.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartEstate.Email;

namespace SmartEstate.Email;

public class SmtpEmailService : IEmailService, IDisposable
{
    public bool EnableSsl { get; set; } = true;
    private readonly SmtpSettings _settings;
    private readonly SmtpClient _client;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<SmtpSettings> settings,
        ILogger<SmtpEmailService> logger)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            
        _logger = logger;
        _client = new SmtpClient(_settings.Host, _settings.Port)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
            EnableSsl = _settings.EnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        _logger.LogInformation("SMTP client configured for {Host}", _settings.Host);
    }


    public async Task SendPasswordRecoveryEmailAsync(string email, string token)
    {
        
        var recoveryLink = $"http://smartestate:5001/password-recovery/confirm?token={token}";
        var subject = "Восстановление пароля";

        await SendEmailAsync(email, subject, EmailTemplates.GetPasswordRecoveryTemplate(recoveryLink));
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            
            await _client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
            throw;
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }

}
