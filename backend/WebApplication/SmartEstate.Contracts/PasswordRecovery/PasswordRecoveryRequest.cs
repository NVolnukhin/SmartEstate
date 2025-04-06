using System.ComponentModel.DataAnnotations;

namespace Contracts.PasswordRecovery;

public record PasswordRecoveryRequest(
    [property: Required, EmailAddress] string Email);