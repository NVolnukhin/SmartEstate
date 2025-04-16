using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record UpdateEmailRequest(
    [Required(ErrorMessage = "Требуется email")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    [StringLength(100, ErrorMessage = "Email слишком длинный")]
    string NewEmail
);
