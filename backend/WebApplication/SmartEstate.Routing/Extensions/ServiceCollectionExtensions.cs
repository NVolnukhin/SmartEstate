using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartEstate.Routing.Models;
using SmartEstate.Routing.Services;
using SmartEstate.Routing.Services.Interfaces;

namespace SmartEstate.Routing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRoutingModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GraphHopperOptions>(configuration.GetSection("GraphHopper"));
            services.AddHttpClient<IRoutingService, RoutingService>();
            
            return services;
        }
    }
}