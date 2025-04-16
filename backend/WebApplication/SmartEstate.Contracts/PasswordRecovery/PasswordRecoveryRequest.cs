using System.ComponentModel.DataAnnotations;

namespace Contracts.PasswordRecovery;

public sealed record PasswordRecoveryRequest(
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    string Email
);