using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SmartEstate.Routing.Models;
using SmartEstate.Routing.Services.Interfaces;

namespace SmartEstate.Routing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutingController : ControllerBase
    {
        private readonly IRoutingService _routingService;

        public RoutingController(IRoutingService routingService)
        {
            _routingService = routingService;
        }

        /// <summary>
        /// Получить время пешеходного маршрута между точками
        /// </summary>
        [HttpGet("walking-time")]
        public async Task<ActionResult<WalkingTimeResponse>> GetWalkingTime([FromQuery] RoutingRequest request)
        {
            var result = await _routingService.GetWalkingRouteAsync(request);
            var path = result.Paths?.FirstOrDefault();
    
            return path == null 
                ? NotFound("Route not found") 
                : Ok(WalkingTimeResponse.FromPath(path));
        }
    }
}