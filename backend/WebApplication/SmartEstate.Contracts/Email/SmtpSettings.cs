using System.ComponentModel.DataAnnotations;

namespace Contracts.Email;

public sealed record SmtpSettings(
    [property: Required(ErrorMessage = "SMTP Host является обязательным")]
    string Host,
    [property: Range(1, 65535, ErrorMessage = "Порт должен быть в диапазоне 1-65535")]
    int Port,
    bool EnableSsl,
    [property: Required(ErrorMessage = "Имя пользователя обязательно")]
    string UserName,
    string Password,
    [property: Required(ErrorMessage = "Email отправителя обязателен")]
    [property: EmailAddress(ErrorMessage = "Некорректный формат email отправителя")]
    string FromEmail,
    string FromName)
{
public SmtpSettings() : this("", 0, true, "", "", "", "") { }

public bool IsValid() => 
    !string.IsNullOrWhiteSpace(Host) &&
    Port > 0 &&
    !string.IsNullOrWhiteSpace(UserName) &&
    !string.IsNullOrWhiteSpace(FromEmail);
}

