using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts.Users;
using SmartEstate.Application.Services;

namespace Presentation.Endpoints;

[ApiController]
public static class UserEndpoint
{
    public static IEndpointRouteBuilder MapUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", Register);
        app.MapPost("login", Login);
        
        app.MapPut("users/{userId:guid}/email", UpdateEmail).RequireAuthorization();
        app.MapPut("users/{userId:guid}/name", UpdateName).RequireAuthorization();
        app.MapPut("users/{userId:guid}/password", UpdatePassword).RequireAuthorization();
        
        return app;
    }
    
    private static async Task<IResult> Register(
        [FromBody] RegisterUserRequest request,
        UserService userService)
    {
        var result = await userService.Register(request.Login, request.Email, request.Password, request.Name);
    
        if (result.IsFailed)
        {
            return Results.BadRequest(new { errors = result.Errors });
        }
    
        return Results.Ok(new { user = result.Value });
    }
    
    private static async Task<IResult> Login(
        [FromBody] LoginUserRequest request,
        UserService userService,
        HttpContext httpContext)
    {
        var result = await userService.Login(request.Login, request.Password);
    
        if (result.IsFailed)
        {
            return Results.BadRequest(new { error = result.Errors.First().Message });
        }

        httpContext.Response.Cookies.Append("whtstht", result.Value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(12)
        });
    
        return Results.Ok(new { token = result.Value });
    }
    
    private static async Task<IResult> UpdateEmail(
        Guid userId,
        [FromBody] UpdateEmailRequest request,
        UserService userService)
    {
        var result = await userService.UpdateEmail(userId, request.NewEmail);
        return result.IsFailed ? Results.BadRequest(result.Errors) : Results.Ok();
    }

    private static async Task<IResult> UpdateName(
        Guid userId,
        [FromBody] UpdateNameRequest request,
        UserService userService)
    {
        var result = await userService.UpdateName(userId, request.NewName);
        return result.IsFailed ? Results.BadRequest(result.Errors) : Results.Ok();
    }

    private static async Task<IResult> UpdatePassword(
        Guid userId,
        [FromBody] UpdatePasswordRequest request,
        UserService userService)
    {
        var result = await userService.UpdatePassword(userId, request.NewPassword, request.CurrentPassword);
        return result.IsFailed ? Results.BadRequest(result.Errors) : Results.Ok();
    }

}