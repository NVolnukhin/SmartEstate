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
            // Пробуем найти по логину ИЛИ email (так как пользователь может вводить и то, и другое)
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
}