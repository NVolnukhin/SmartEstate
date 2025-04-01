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
        
        /*[HttpGet("test")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var testUrl = $"{_options.BaseUrl}route?point=55.756808,37.621984&point=55.754741,37.628406&vehicle=foot&key={_options.ApiKey}";
                var response = await _httpClient.GetAsync(testUrl);
                 
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
 
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }*/
    }
}