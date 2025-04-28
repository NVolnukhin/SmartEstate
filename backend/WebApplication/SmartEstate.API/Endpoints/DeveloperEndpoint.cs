using Contracts.Developer;
using Contracts.InfrastructureInfo;
using Microsoft.AspNetCore.Mvc;
using SmartEstate.Application.Services;

namespace Presentation.Endpoints;

[ApiController]
[Route("api/developer")]
public class DeveloperEndpoint : ControllerBase
{
    private readonly IDeveloperService _developerService;

    public DeveloperEndpoint(IDeveloperService developerService)
    {
        _developerService = developerService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ListDeveloperDto>>> GetAllMetroStations()
    {
        try
        {
            var result = await _developerService.GetAllDevelopersAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}