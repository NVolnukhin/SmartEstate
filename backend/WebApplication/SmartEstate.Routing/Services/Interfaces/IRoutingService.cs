using System.Threading.Tasks;
using SmartEstate.Routing.Models;

namespace SmartEstate.Routing.Services.Interfaces
{
    public interface IRoutingService
    {
        Task<RoutingResponse> GetWalkingRouteAsync(RoutingRequest request);
    }
}