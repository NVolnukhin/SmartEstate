using System.ComponentModel.DataAnnotations;

namespace Contracts.PasswordRecovery;

public record ForgotPasswordRequestDto(
    [property: Required, EmailAddress] string Email);