using System;
using System.Threading.Tasks;
using DatabaseModel;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts.Users;

namespace SmartEstate.Application.Services;

public interface IUserService
{
    public Task<Result<(User User, string Token)>> Register(string login, string email, string password, string name);
    public Task<Result<string>> Login(string loginOrEmail, string password);
    public Task<UserInfoResponse> GetUserInfo(Guid userId);
    public Task<Result> UpdateEmail(Guid userId, string newEmail);
    public Task<Result> UpdateName(Guid userId, string newName);
    public Task<Result> UpdatePassword(Guid userId, string newPassword, string currentPassword);
    public Task<Result> RecoverPassword(Guid userId, string newPassword, string confirmPassword);
}