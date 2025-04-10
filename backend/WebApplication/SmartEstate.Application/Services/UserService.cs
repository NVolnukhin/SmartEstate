using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseModel;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Contracts.Users;
using SmartEstate.Application.Interfaces;
using SmartEstate.DataAccess.Repositories;

namespace SmartEstate.Application.Services;


public class UserService : IUserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordRecoveryTokenRepository _recoveryTokenRepository;
    
    public UserService(
        IUsersRepository usersRepository,
        IPasswordRecoveryTokenRepository recoveryTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        ILogger<UserService> logger)
    {
        _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        _recoveryTokenRepository = recoveryTokenRepository ?? throw new ArgumentNullException(nameof(recoveryTokenRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtProvider = jwtProvider ?? throw new ArgumentNullException(nameof(jwtProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task<Result<(User User, string Token)>> Register(
        string login,
        string email,
        string password,
        string name)
    {
        try
        {
            var validationResult = ValidateUserData(login, email, password, name);
            if (validationResult.IsFailed)
                return validationResult;
            
            if (await _usersRepository.GetByEmail(email) != null)
                return Result.Fail("Пользователь с таким email уже существует");

            if (await _usersRepository.GetByLogin(login) != null)
                return Result.Fail("Пользователь с таким логином уже существует");
            
            var hashedPassword = _passwordHasher.Generate(password);
            var user = User.Create(
                Guid.NewGuid(),
                login.Trim(),
                email.Trim().ToLower(),
                hashedPassword,
                name.Trim());

            await _usersRepository.Add(user);
            _logger.LogInformation("User registered with ID: {UserId}", user.UserId);

            var token = _jwtProvider.GenerateToken(user);
            return Result.Ok((user, token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return Result.Fail("Произошла ошибка при регистрации");
        }
    }

    public async Task<Result<string>> Login(string loginOrEmail, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loginOrEmail) || string.IsNullOrWhiteSpace(password))
                return Result.Fail("Логин/email и пароль обязательны");

            var user = await _usersRepository.GetByLogin(loginOrEmail) ??
                       await _usersRepository.GetByEmail(loginOrEmail);

            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Login}", loginOrEmail);
                return Result.Fail("Неверные учетные данные");
            }

            var passwordValid = _passwordHasher.Verify(password, user.HashedPassword);
            if (!passwordValid)
            {
                _logger.LogWarning("Invalid password attempt for user: {UserId}", user.UserId);
                return Result.Fail("Неверные учетные данные");
            }

            var token = _jwtProvider.GenerateToken(user);
            _logger.LogInformation("User logged in: {UserId}", user.UserId);
            return Result.Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return Result.Fail("Произошла ошибка при входе");
        }
    }

    public async Task<UserInfoResponse> GetUserInfo(Guid userId)
    {
        var user = await _usersRepository.GetById(userId);
        return new UserInfoResponse(user.Login, user.Name ?? "", user.Email);
    }
    
    
    public async Task<Result> UpdateEmail(Guid userId, string newEmail)
    {
        try
        {
            if (!new EmailAddressAttribute().IsValid(newEmail))
            {
                _logger.LogWarning("Invalid email format attempt for user {UserId}: {Email}", userId, newEmail);
                return Result.Fail("Некорректный формат email");
            }

            var existingUser = await _usersRepository.GetByEmail(newEmail);
            if (existingUser != null && existingUser.UserId != userId)
            {
                _logger.LogWarning("Email conflict for user {UserId}: {Email} already taken", userId, newEmail);
                return Result.Fail("Этот email уже используется");
            }

            await _usersRepository.UpdateEmail(userId, newEmail);
            _logger.LogInformation("Email updated for user {UserId}", userId);
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating email for user {UserId}", userId);
            return Result.Fail("Произошла ошибка при обновлении email");
        }
    }

    public async Task<Result> UpdateName(Guid userId, string newName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newName) || newName.Length < 3)
            {
                _logger.LogWarning("Invalid name update attempt for user {UserId}: {Name}", userId, newName);
                return Result.Fail("Имя должно содержать минимум 3 символа");
            }

            await _usersRepository.UpdateName(userId, newName.Trim());
            _logger.LogInformation("Name updated for user {UserId}", userId);
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating name for user {UserId}", userId);
            return Result.Fail("Произошла ошибка при обновлении имени");
        }
    }

    public async Task<Result> UpdatePassword(Guid userId, string newPassword, string currentPassword)
    {
        try
        {
            var user = await _usersRepository.GetById(userId);
            if (user == null)
            {
                _logger.LogWarning("Password update attempt for non-existent user {UserId}", userId);
                return Result.Fail("Пользователь не найден");
            }

            if (!_passwordHasher.Verify(currentPassword, user.HashedPassword))
            {
                _logger.LogWarning("Invalid current password attempt for user {UserId}", userId);
                return Result.Fail("Неверный текущий пароль");
            }

            var newPasswordHash = _passwordHasher.Generate(newPassword);
            await _usersRepository.UpdatePassword(userId, newPasswordHash);
            
            _logger.LogInformation("Password updated for user {UserId}", userId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password for user {UserId}", userId);
            return Result.Fail("Произошла ошибка при обновлении пароля");
        }
    }
    
    public async Task<Result> RecoverPassword(Guid userId, string newPassword, string confirmPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return Result.Fail("Новый пароль не может быть пустым");
            }

            if (!newPassword.Equals(confirmPassword))
            {
                return Result.Fail("Пароли не совпадают");
            }

            var user = await _usersRepository.GetById(userId);
            if (user == null)
            {
                _logger.LogWarning("Попытка восстановления пароля для несуществующего пользователя {UserId}", userId);
                return Result.Fail("Пользователь не найден");
            }
            
            var hashedPassword = _passwordHasher.Generate(newPassword);
            await _usersRepository.UpdatePassword(userId, hashedPassword);
            await _recoveryTokenRepository.InvalidateUserTokensAsync(userId);
        
            _logger.LogInformation("Пароль успешно обновлён для пользователя {UserId}", userId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при восстановлении пароля для пользователя {UserId}", userId);
            return Result.Fail("Произошла ошибка при восстановлении пароля");
        }
    }
    
    
    private Result ValidateUserData(string login, string name)
    {
        if (string.IsNullOrWhiteSpace(login) || login.Length < 3)
            return Result.Fail("Логин должен содержать минимум 3 символа");

        if (!new EmailAddressAttribute().IsValid(email))
            return Result.Fail("Некорректный формат email");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return Result.Fail("Пароль должен содержать минимум 8 символов");
        
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            return Result.Fail("Имя должно содержать минимум 2 символа");

        return Result.Ok();
    }
}