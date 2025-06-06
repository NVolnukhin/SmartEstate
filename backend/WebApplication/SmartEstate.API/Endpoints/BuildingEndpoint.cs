using Presentation.Contracts.Building;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts.Flats;
using SmartEstate.Application.Services;

namespace Presentation.Endpoints;

[ApiController]
[Route("api/buildings")]
public class BuildingEndpoint : ControllerBase
{
    private readonly IBuildingService _buildingService;

    public BuildingEndpoint(IBuildingService buildingService)
    {
        _buildingService = buildingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BuildingMapDto>>> GetAllBuildings()
    {
        try
        {
            var result = await _buildingService.GetAllBuildingsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{buildingId:int}/flats")]
    public async Task<ActionResult<List<FlatShortInfoResponse>>> GetFlatsByBuildingId(int buildingId)
    {
        try
        {
            var result = await _buildingService.GetFlatsByBuildingIdAsync(buildingId);
            return result == null || !result.Any() ? NotFound() : Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}