namespace SmartEstate.Email;

public interface IEmailService
{
    Task SendPasswordRecoveryEmailAsync(string email, string token);
    Task SendWelcomeEmailAsync(string email);
    Task SendChangeEmailAsync(string email, string userName);
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}
