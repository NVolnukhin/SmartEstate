using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record UpdatePasswordRequest(
    [Required(ErrorMessage = "Требуется текущий пароль")]
    string CurrentPassword,

    [Required(ErrorMessage = "Требуется новый пароль")]
    [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", 
        ErrorMessage = "Пароль должен содержать цифры, заглавные и строчные буквы")]
    string NewPassword
);