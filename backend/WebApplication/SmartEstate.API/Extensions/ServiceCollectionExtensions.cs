using DatabaseContext.Repositories;
using SmartEstate.Application.Interfaces;
using SmartEstate.Application.Services;
using SmartEstate.ApplicationServices;
using SmartEstate.DataAccess.Repositories;
using SmartEstate.Email;
using SmartEstate.Infrastructure;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IFlatsRepository, FlatsRepository>();
        services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();
        services.AddScoped<IFlatService, FlatService>();
        services.AddTransient<IEmailService, SmtpEmailService>();
        services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();
        services.AddScoped<IPasswordRecoveryTokenRepository, PasswordRecoveryTokenRepository>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });
        return services;
    }
}
