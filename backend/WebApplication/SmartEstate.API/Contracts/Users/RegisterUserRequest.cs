using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record RegisterUserRequest(
    [Required] string Login,
    [Required][EmailAddress] string Email,
    [Required] string Name,
    [Required][MinLength(8)] string Password);