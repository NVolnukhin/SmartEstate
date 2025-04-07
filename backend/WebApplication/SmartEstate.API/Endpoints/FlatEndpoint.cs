using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
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
    public async Task<ActionResult<PagedResponse<FlatResponse>>> GetAllFlats([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
    {
        var result = await _flatService.GetAllFlatsAsync(page, pageSize);
        return Ok(result);
    }


    [HttpGet("random")]
    public async Task<ActionResult<List<FlatShortInfoResponse>>> GetRandomFlats()
    {
        var result = await _flatService.GetRandomFlatsAsync();
        return Ok(result);
    }

    [HttpGet("{flatId:int}")]
    public async Task<ActionResult<FlatDetailsResponse>> GetFlatById(int flatId)
    {
        var result = await _flatService.GetFlatDetailsByIdAsync(flatId);
        return result == null ? NotFound() : Ok(result);
    }
}