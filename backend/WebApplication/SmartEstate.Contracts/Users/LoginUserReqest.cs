using System.ComponentModel.DataAnnotations;

namespace Presentation.Contracts.Users;

public record LoginUserRequest(
    [Required(ErrorMessage = "Логин или email обязателен")] string Login,
    [Required][Length(64, 64)] string Password); //64 - len of sha256 hash 