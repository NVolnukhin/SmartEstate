using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record RegisterUserRequest(
    [Required][MinLength(5)] string Login,
    [Required][EmailAddress] string Email,
    [Required][MinLength(3)] string Name,
    [Required][MinLength(8)] string Password);