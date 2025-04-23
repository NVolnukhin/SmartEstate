// MetroEndpoint.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.InfrastructureInfo;
using Microsoft.AspNetCore.Mvc;
using SmartEstate.Application.Services;

namespace Presentation.Endpoints;

[ApiController]
[Route("api/metro")]
public class MetroEndpoint : ControllerBase
{
    private readonly IMetroService _metroService;

    public MetroEndpoint(IMetroService metroService)
    {
        _metroService = metroService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<MetroDto>>> GetAllMetroStations()
    {
        try
        {
            var result = await _metroService.GetAllMetroStationsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}