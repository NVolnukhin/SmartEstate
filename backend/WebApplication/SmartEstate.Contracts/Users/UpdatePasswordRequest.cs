using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record UpdatePasswordRequest(
    [Required][Length(64, 64)]
    string CurrentPassword,
    [Required][Length(64, 64)]
    string NewPassword
);