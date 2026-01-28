using TecLocalizer.DAL.Repositories.Interfaces; 
using TecLocalizer.DL.Models;               
using Microsoft.EntityFrameworkCore; 

namespace TecLocalizer.DAL.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly DAL.Repositories.TecDbContext _context;

    public VehicleRepository(DAL.Repositories.TecDbContext context)
    {
        _context = context;
    }

    public async Task<List<DL.Models.VehiclePosition>> GetLiveVehiclesAsync()
    {
        // Simulation
        return await Task.FromResult(new List<DL.Models.VehiclePosition>
        {
            new DL.Models.VehiclePosition 
            { 
                VehicleId = "BUS001", 
                RouteId = "R1", 
                Latitude = 50.6332, 
                Longitude = 5.5651, 
                Speed = 25.5, 
                UpdatedAt = DateTime.UtcNow 
            },
            new DL.Models.VehiclePosition 
            { 
                VehicleId = "BUS002", 
                RouteId = "R2", 
                Latitude = 50.6371, 
                Longitude = 5.5672, 
                Speed = 18.2, 
                UpdatedAt = DateTime.UtcNow 
            }
        });
    }
}