using System.ComponentModel.DataAnnotations;

namespace Contracts.PasswordRecovery;

public record PasswordRecoveryConfirm(
    [property: Required] string Token,
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(8)] string NewPassword,
    [property: Compare("NewPassword")] string ConfirmPassword
);