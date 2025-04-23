using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts.Users;
using SmartEstate.Application.Services;

namespace Presentation.Endpoints;

[ApiController]
[Route("api/users")]
public class UsersEndpoint : ControllerBase
{
    private readonly IUserService _userService;

    public UsersEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var result = await _userService.Register(request.Login, request.Email, request.Password, request.Name);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            var (user, token) = result.Value;

            Response.Cookies.Append("whtstht", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(12)
            });

            return Ok(new
            {
                user.UserId,
                user.Email,
                user.Name,
                Token = token
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var result = await _userService.Login(request.Login, request.Password);

            if (result.IsFailed)
                return BadRequest(new { error = result.Errors.First().Message });

            Response.Cookies.Append("whtstht", result.Value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(12)
            });

            return Ok(new { token = result.Value });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var information = await _userService.GetUserInfo(userId);
            return Ok(information);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal Server Error: " + ex.Message);
        }
    }
    
    [HttpPut("email")]
    [Authorize]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _userService.UpdateEmail(userId, request.NewEmail);
            return result.IsFailed ? BadRequest(result.Errors) : Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("name")]
    [Authorize]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _userService.UpdateName(userId, request.NewName);
            return result.IsFailed ? BadRequest(result.Errors) : Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var result = await _userService.UpdatePassword(userId, request.NewPassword, request.CurrentPassword);
            return result.IsFailed ? BadRequest(result.Errors) : Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    private Guid GetUserIdFromClaims()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userId ?? throw new UnauthorizedAccessException("User ID not found in claims"));
    }
}
