using TecLocalizer.BLL.DTOs;
using TecLocalizer.DL.Enums;

namespace TecLocalizer.BLL.Services.Interfaces;

public interface IVehiclePositionService
{
    Task<List<VehicleDto>> GetCurrentPositionsAsync(Province? province = null);
    Task StartPollingAsync(CancellationToken cancellationToken);
    Task StopPollingAsync();
    DateTime LastUpdateTime { get; }
}
