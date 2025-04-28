using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record UpdateNameRequest(
    [Required][MinLength(3)] string NewName);