using System;
using DatabaseModel;

namespace DatabaseModels.RecoveryPassword;

public class PasswordRecoveryToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    
    public virtual User User { get; set; }
}