using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using Contracts.Filters;
using Contracts.Flats;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts.Flats;
using SmartEstate.Application.Services;

namespace Presentation.Endpoints;

[ApiController]
[Route("api/flats")]
public class FlatEndpoint : ControllerBase
{
    private readonly IFlatService _flatService;

    public FlatEndpoint(IFlatService flatService)
    {
        _flatService = flatService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<FlatResponse>>> GetAllFlats(
        [FromQuery] string? order = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] string? roominess = null,
        [FromQuery] string? metroStations = null,
        [FromQuery] int? maxMetroTime = null,
        [FromQuery] string? developers = null,
        [FromQuery] int? minFloor = null,
        [FromQuery] int? maxFloor = null,
        [FromQuery] int? minFloorCount = null,
        [FromQuery] int? maxFloorCount = null,
        [FromQuery] string? buildingStatus = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] decimal? minSquare = null,
        [FromQuery] decimal? maxSquare = null)
    {
        var filters = new FlatFilterRequest(
            order,
            minPrice,
            maxPrice,
            minSquare,
            maxSquare,
            minFloor,
            maxFloor,
            minFloorCount,
            maxFloorCount,
            maxMetroTime,
            roominess?.Split(',').Select(int.Parse).ToList(),
            metroStations?.Split(',').Select(int.Parse).ToList(),
            developers?.Split(',').Select(int.Parse).ToList(),
            buildingStatus?.Split(',').ToList());
        
        var result = await _flatService.GetAllFlatsAsync(page, pageSize, filters);
        return Ok(result);
    }


    [HttpGet("random")]
    public async Task<ActionResult<List<FlatShortInfoResponse>>> GetRandomFlats([FromQuery] int count = 10)
    {
        var result = await _flatService.GetRandomFlatsAsync(count);
        return Ok(result);
    }

    [HttpGet("{flatId:int}")]
    public async Task<ActionResult<FlatDetailsResponse>> GetFlatById(int flatId)
    {
        var result = await _flatService.GetFlatDetailsByIdAsync(flatId);
        return result == null ? NotFound() : Ok(result);
    }
}