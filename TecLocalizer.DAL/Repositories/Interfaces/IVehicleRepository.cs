using TecLocalizer.DL.Models;

namespace TecLocalizer.DAL.Repositories.Interfaces;

public interface IVehicleRepository
{
    Task<List<VehiclePosition>> GetLiveVehiclesAsync();
}