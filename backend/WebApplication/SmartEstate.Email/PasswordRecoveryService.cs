using System;
using System.Threading.Tasks;
using Contracts.PasswordRecovery;
using DatabaseModel;
using DatabaseModels.RecoveryPassword;
using FluentResults;
using Microsoft.Extensions.Logging;
using SmartEstate.Application.Interfaces;
using SmartEstate.DataAccess.Repositories;
using SmartEstate.Email;

namespace SmartEstate.Application.Services
{
    public class PasswordRecoveryService : IPasswordRecoveryService
    {
        private readonly IPasswordRecoveryTokenRepository _tokenRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<PasswordRecoveryService> _logger;
        private readonly IUserService _userService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IEmailEncryptor _emailEncryptor;


        public PasswordRecoveryService(
            IPasswordRecoveryTokenRepository tokenRepository,
            IUsersRepository usersRepository,
            ILogger<PasswordRecoveryService> logger,
            IUserService userService,
            ITokenGenerator tokenGenerator,
            IEmailService emailService,
            IEmailEncryptor emailEncryptor)
        {
            _tokenRepository = tokenRepository;
            _usersRepository = usersRepository;
            _logger = logger;
            _userService = userService;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
            _emailEncryptor = emailEncryptor;
        }

        public async Task<Result<PasswordRecoveryResponse>> RequestRecoveryAsync(string encryptedEmail)
        {
            try
            {
                var user = await _usersRepository.GetByEmail(encryptedEmail);
                if (user is null)
                {
                    return Result.Ok(new PasswordRecoveryResponse(
                        true,
                        "Если аккаунт с таким email существует, инструкции будут отправлены"));
                }

                await _tokenRepository.InvalidateUserTokensAsync(user.UserId);

                var token = new PasswordRecoveryToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.UserId,
                    Token = _tokenGenerator.GenerateToken(),
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    IsUsed = false
                };

                await _tokenRepository.CreateTokenAsync(token);

                try
                {
                    var email = _emailEncryptor.Decrypt(user.Email);
                    await _emailService.SendPasswordRecoveryEmailAsync(email, token.Token);
                    return Result.Ok(new PasswordRecoveryResponse(
                        true,
                        "Если аккаунт с таким email существует, письмо отправлено"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending recovery email");
                    return Result.Fail("Ошибка при отправке письма с инструкциями");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting password recovery");
                return Result.Fail("Ошибка при запросе восстановления пароля");
            }
        }

        public async Task<Result<PasswordRecoveryResponse>> ConfirmRecoveryAsync(PasswordRecoveryConfirm request)
        {
            try
            {
                var token = await _tokenRepository.GetValidTokenAsync(request.Token);
                if (token is null)
                {
                    return Result.Fail("Недействительная или просроченная ссылка");
                }

                var result = await _userService.RecoverPassword(
                    token.UserId,
                    request.NewPassword,
                    request.ConfirmPassword);

                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                token.IsUsed = true;
                await _tokenRepository.CreateTokenAsync(token);

                _logger.LogInformation("Password recovered for user {UserId}", token.UserId);

                return Result.Ok(new PasswordRecoveryResponse(
                    true,
                    "Пароль успешно изменен"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming password recovery");
                return Result.Fail("Ошибка при восстановлении пароля");
            }
        }
    }
}