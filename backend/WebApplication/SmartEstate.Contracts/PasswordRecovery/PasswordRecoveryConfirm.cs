using System.ComponentModel.DataAnnotations;

public sealed record PasswordRecoveryConfirm
{
    [Required(ErrorMessage = "Токен обязателен")]
    public string Token { get; init; }
    
    [Required][Length(64, 64)]
    public string NewPassword { get; init; }
    
    [Required][Length(64, 64)]
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