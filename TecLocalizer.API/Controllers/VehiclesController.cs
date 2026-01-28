using Microsoft.AspNetCore.Mvc;
using TecLocalizer.DAL.Repositories.Interfaces;
using TecLocalizer.API.DTOs;
using TecLocalizer.DL.Models;

namespace TecLocalizer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly TecLocalizer.DAL.Repositories.Interfaces.IVehicleRepository _repository;

    public VehiclesController(TecLocalizer.DAL.Repositories.Interfaces.IVehicleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<TecLocalizer.API.DTOs.VehicleDto>>> Get()
    {
        var vehicles = await _repository.GetLiveVehiclesAsync();
        
        var dtos = vehicles.Select(v => new TecLocalizer.API.DTOs.VehicleDto
        {
            VehicleId = v.VehicleId,
            RouteShortName = "LIGNE",
            Latitude = v.Latitude,
            Longitude = v.Longitude,
            Speed = v.Speed
        }).ToList();

        return Ok(dtos);
    }
}