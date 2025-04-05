using Presentation.Contracts.Users;
using SmartEstate.Application.Services;

namespace Presentation.Endpoints;

public static class UserEndpoint
{
    public static IEndpointRouteBuilder MapUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", Register);

        app.MapPost("login", Login);
        
        return app;
    }
    
    private static async Task<IResult> Register(
        RegisterUserRequest request,
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
        LoginUserRequest request,
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
}