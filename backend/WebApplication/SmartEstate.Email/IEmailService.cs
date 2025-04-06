namespace SmartEstate.Email;

public interface IEmailService
{
    Task SendPasswordRecoveryEmailAsync(string email, string token);
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}
