using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts.PasswordRecovery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartEstate.Application.Interfaces;
using SmartEstate.Email;

[ApiController]
[Route("api/password-recovery")]
public class PasswordRecoveryController : ControllerBase
{
    private readonly IPasswordRecoveryService _recoveryService;
    private readonly ILogger<PasswordRecoveryController> _logger;

    public PasswordRecoveryController(
        IPasswordRecoveryService recoveryService,
        ILogger<PasswordRecoveryController> logger)
    {
        _recoveryService = recoveryService;
        _logger = logger;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestRecovery(
        [FromBody] PasswordRecoveryRequest request)
    {
        try
        {
            var result = await _recoveryService.RequestRecoveryAsync(request.Email);
            
            if (!result.IsSuccess)
            {
                var errorResponse = new ErrorResponse
                {
                    Message = "Ошибка при запросе восстановления пароля",
                    Errors = result.Errors.Select(e => 
                        new ErrorResponse.ErrorDetail("RECOVERY_ERROR", e.Message)).ToList()
                };
                
                _logger.LogWarning("Ошибка восстановления: {Email} - {Errors}", 
                    request.Email, string.Join(", ", result.Errors));
                
                return BadRequest(errorResponse);
            }
            
            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанная ошибка при восстановлении пароля");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Внутренняя ошибка сервера",
                Errors = { new("SERVER_ERROR", "Произошла непредвиденная ошибка") }
            });
        }
    }
    
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmRecovery(
        [FromBody] PasswordRecoveryConfirm request)
    {
        _logger.LogInformation("ConfirmRecovery request: {@Request}", request);
        try
        {
            var result = await _recoveryService.ConfirmRecoveryAsync(request);
            
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service errors: {Errors}", string.Join(", ", result.Errors));
            }
            
            return result.IsSuccess 
                ? Ok(result.Value) 
                : BadRequest(new ErrorResponse
                {
                    Message = "Ошибка при подтверждении восстановления",
                    Errors = result.Errors.Select(e => 
                        new ErrorResponse.ErrorDetail("CONFIRM_ERROR", e.Message)).ToList()
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConfirmRecovery error");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Внутренняя ошибка сервера",
                Errors = { new("SERVER_ERROR", "Произошла непредвиденная ошибка") }
            });
        }
    }
}