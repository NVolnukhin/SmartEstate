using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record LoginUserRequest(
    [Required(ErrorMessage = "Логин или email обязателен")] string Login,
    [Required(ErrorMessage = "Пароль обязателен")] string Password);