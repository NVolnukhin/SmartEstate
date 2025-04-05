using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using DatabaseModel;
using FluentResults;
using SmartEstate.Application.Interfaces;
using SmartEstate.DataAccess.Repositories;

namespace SmartEstate.Application.Services;

public class UserService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserService(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher, 
        IJwtProvider jwtProvider)
    {
        _passwordHasher = passwordHasher;
        _usersRepository = usersRepository;
        _jwtProvider = jwtProvider;
    }
    
    
    public async Task<Result<User>> Register(string login, string email, string password, string name)
    {
        try
        {
            var userByEmail = await _usersRepository.GetByEmail(email);
            if (userByEmail != null)
            {
                return Result.Fail<User>("Пользователь с таким email уже существует");
            }

            var userByLogin = await _usersRepository.GetByLogin(login);
            if (userByLogin != null)
            {
                return Result.Fail<User>("Пользователь с таким логином уже существует");
            }
        
            var hashedPassword = _passwordHasher.Generate(password); 
        
            var user = User.Create(
                id: Guid.NewGuid(),
                login: login.Trim(),
                email: email.Trim().ToLower(),
                passwordHash: hashedPassword,
                name: name.Trim());

            await _usersRepository.Add(user);
    
            return Result.Ok(user);
        }
        catch (Exception ex)
        {
            return Result.Fail<User>(ex.Message);
        }
    }

    public async Task<Result<string>> Login(string login, string password)
    {
        try
        {
            var user = await _usersRepository.GetByLogin(login) ?? 
                       await _usersRepository.GetByEmail(login);

            if (user == null)
            {
                return Result.Fail<string>("Пользователь с таким логином/email не найден");
            }

            var passwordValid = _passwordHasher.Verify(password, user.HashedPassword);
            if (!passwordValid)
            {
                return Result.Fail<string>("Неверный пароль");
            }

            var token = _jwtProvider.GenerateToken(user);
            return Result.Ok(token);
        }
        catch (Exception ex)
        {
            return Result.Fail<string>(ex.Message);
        }
    }
    
    public async Task<Result> UpdateEmail(Guid userId, string newEmail)
    {
        var user = await _usersRepository.GetById(userId);
        
        if (await _usersRepository.GetByEmail(newEmail) != null)
        {
            return Result.Fail("Этот email уже используется");
        }
        
        if (!new EmailAddressAttribute().IsValid(newEmail))
        {
            return Result.Fail("Некорректный формат email");
        }
        
        await _usersRepository.UpdateEmail(userId, newEmail);
        return Result.Ok();
    }

    public async Task<Result> UpdateName(Guid userId, string newName)
    {
        if (newName.Length < 3)
        {
            return Result.Fail("Имя должно быть длиннее 3 символов");
        }
        await _usersRepository.UpdateName(userId, newName);
        return Result.Ok();
    }

    public async Task<Result> UpdatePassword(Guid userId, string newPassword, string currentPassword)
    {
        var user = await _usersRepository.GetById(userId);
        if (!_passwordHasher.Verify(currentPassword, user.HashedPassword))
        {
            return Result.Fail("Неверный текущий пароль");
        }
        
        if (newPassword.Length < 8)
        {
            return Result.Fail("Пароль должен содержать минимум 8 символов");
        }

        if (!Regex.IsMatch(newPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)"))
        {
            return Result.Fail("Пароль должен содержать цифры, заглавные и строчные буквы");
        }
        
        var newPasswordHash = _passwordHasher.Generate(newPassword);
        
        await _usersRepository.UpdatePassword(userId, newPasswordHash);
        return Result.Ok();
    }
}