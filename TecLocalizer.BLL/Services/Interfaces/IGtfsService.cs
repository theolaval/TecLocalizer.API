using TecLocalizer.BLL.DTOs;
using TecLocalizer.DL.Enums;

namespace TecLocalizer.BLL.Services.Interfaces;

public interface IGtfsService
{
    Task<List<StopDto>> GetAllStopsAsync(Province? province = null);
    Task<List<VehicleDto>> GetVehiclePositionsAsync(Province? province = null);
    Task InitializeAsync();
    DateTime LastUpdateTime { get; }
}
