using Microsoft.AspNetCore.Mvc;
using TecLocalizer.BLL.DTOs;
using TecLocalizer.BLL.Services.Interfaces;
using TecLocalizer.DL.Enums;

namespace TecLocalizer.API.Controllers;

/// <summary>
/// API REST pour les arrêts de bus
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StopsController : ControllerBase
{
    private readonly IGtfsService _gtfsService;
    private readonly ILogger<StopsController> _logger;

    public StopsController(IGtfsService gtfsService, ILogger<StopsController> logger)
    {
        _gtfsService = gtfsService;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les arrêts, optionnellement filtrés par province
    /// </summary>
    /// <param name="province">Province (Liege, Namur, Hainaut, BrabantWallon, Luxembourg)</param>
    /// <returns>Liste des arrêts</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<StopDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StopDto>>> GetStops([FromQuery] string? province = null)
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

            var stops = await _gtfsService.GetAllStopsAsync(provinceFilter);
            return Ok(stops);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stops");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving stops");
        }
    }

    /// <summary>
    /// Récupère les arrêts d'une province spécifique
    /// </summary>
    /// <param name="province">Code de la province</param>
    /// <returns>Liste des arrêts de la province</returns>
    [HttpGet("{province}")]
    [ProducesResponseType(typeof(List<StopDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StopDto>>> GetStopsByProvince(string province)
    {
        return await GetStops(province);
    }
}
