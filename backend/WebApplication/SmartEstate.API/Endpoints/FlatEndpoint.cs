using System.Collections.Generic;
using System.Threading.Tasks;
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
    public async Task<ActionResult<List<FlatResponse>>> GetAllFlats()
    {
        var result = await _flatService.GetAllFlatsAsync();
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