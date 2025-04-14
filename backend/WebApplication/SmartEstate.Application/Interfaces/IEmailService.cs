namespace SmartEstate.Email;

public interface IEmailService
{
    Task SendPasswordRecoveryEmailAsync(string email, string token);
    Task SendWelcomeEmailAsync(string email);
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}
