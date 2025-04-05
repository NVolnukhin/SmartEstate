using DatabaseContext.Repositories;
using SmartEstate.Application.Interfaces;
using SmartEstate.Application.Services;
using SmartEstate.DataAccess.Repositories;
using SmartEstate.Infrastructure;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<UserService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}
