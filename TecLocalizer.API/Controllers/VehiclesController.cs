using Microsoft.AspNetCore.Mvc;
using TecLocalizer.BLL.DTOs;
using TecLocalizer.BLL.Services.Interfaces;
using TecLocalizer.DL.Enums;

namespace TecLocalizer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private readonly IVehiclePositionService _vehiclePositionService;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IVehiclePositionService vehiclePositionService, ILogger<VehiclesController> logger)
    {
        _vehiclePositionService = vehiclePositionService;
        _logger = logger;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(List<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VehicleDto>>> GetVehicles([FromQuery] string? province = null)
    {
        try
        {
            Province? provinceFilter = null;
            
            if (!string.IsNullOrWhiteSpace(province))
            {
                if (Enum.TryParse<Province>(province, ignoreCase: true, out var parsedProvince))
                {
                    provinceFilter = parsedProvince;
                }
                else
                {
                    return BadRequest($"Invalid province. Valid values: {string.Join(", ", Enum.GetNames(typeof(Province)))}");
                }
            }

            var vehicles = await _vehiclePositionService.GetCurrentPositionsAsync(provinceFilter);
            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vehicle positions");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving vehicle positions");
        }
    }
    
    [HttpGet("{province}")]
    [ProducesResponseType(typeof(List<VehicleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VehicleDto>>> GetVehiclesByProvince(string province)
    {
        return await GetVehicles(province);
    }
    
    [HttpGet("stats/summary")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetStats()
    {
        try
        {
            var allVehicles = await _vehiclePositionService.GetCurrentPositionsAsync();
            
            return Ok(new
            {
                totalVehicles = allVehicles.Count,
                lastUpdate = _vehiclePositionService.LastUpdateTime,
                byProvince = allVehicles
                    .GroupBy(v => v.Province)
                    .Select(g => new { province = g.Key, count = g.Count() })
                    .OrderBy(x => x.province)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving statistics");
        }
    }
}
