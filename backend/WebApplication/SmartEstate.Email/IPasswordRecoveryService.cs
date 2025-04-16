using Contracts.PasswordRecovery;
using FluentResults;

namespace SmartEstate.Application.Interfaces;

public interface IPasswordRecoveryService
{
    Task<Result<PasswordRecoveryResponse>> RequestRecoveryAsync(string email);
    Task<Result<PasswordRecoveryResponse>> ConfirmRecoveryAsync(PasswordRecoveryConfirm request);
}