using Microsoft.AspNetCore.SignalR;
using TecLocalizer.API.DTOs;

namespace TecLocalizer.API.Hubs;

public class VehicleHub : Hub
{
    public async Task SendVehicles(List<VehicleDto> vehicles)
    {
        await Clients.All.SendAsync("VehiclesUpdated", vehicles);
    }
}