using System.ComponentModel.DataAnnotations;

public sealed record PasswordRecoveryConfirm
{
    [Required(ErrorMessage = "Токен обязателен")]
    public string Token { get; init; }
    
    [Required(ErrorMessage = "Новый пароль обязателен")]
    [MinLength(8, ErrorMessage = "Пароль должен содержать минимум 8 символов")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$")]
    public string NewPassword { get; init; }
    
    [Required(ErrorMessage = "Подтверждение пароля обязательно")]
    public string ConfirmPassword { get; init; }

    public PasswordRecoveryConfirm(
        string token,
        string newPassword,
        string confirmPassword)
    {
        Token = token;
        NewPassword = newPassword;
        ConfirmPassword = confirmPassword;
    }
}