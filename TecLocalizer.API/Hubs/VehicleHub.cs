using Microsoft.AspNetCore.SignalR;
using TecLocalizer.BLL.DTOs;
using TecLocalizer.BLL.Services.Interfaces;
using TecLocalizer.DL.Enums;

namespace TecLocalizer.API.Hubs;

/// <summary>
/// SignalR Hub pour les mises à jour en temps réel des positions des véhicules
/// </summary>
public class VehicleHub : Hub
{
    private readonly IVehiclePositionService _vehiclePositionService;
    private readonly ILogger<VehicleHub> _logger;

    public VehicleHub(IVehiclePositionService vehiclePositionService, ILogger<VehicleHub> logger)
    {
        _vehiclePositionService = vehiclePositionService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Obtient les positions actuelles des véhicules
    /// </summary>
    public async Task<List<VehicleDto>> GetCurrentVehicles(string? province = null)
    {
        if (!string.IsNullOrWhiteSpace(province))
        {
            if (Enum.TryParse<Province>(province, ignoreCase: true, out var parsedProvince))
            {
                return await _vehiclePositionService.GetCurrentPositionsAsync(parsedProvince);
            }
        }

        return await _vehiclePositionService.GetCurrentPositionsAsync();
    }

    /// <summary>
    /// S'abonne aux mises à jour de positions (appelé par le client)
    /// </summary>
    public async Task SubscribeToUpdates(string? province = null)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"vehicles-{province ?? "all"}");
        _logger.LogDebug("Client subscribed to vehicles updates: {Province}", province ?? "all");
        
        // Send current positions immediately
        var currentVehicles = await GetCurrentVehicles(province);
        await Clients.Caller.SendAsync("VehiclesUpdated", currentVehicles);
    }

    /// <summary>
    /// Se désabonne des mises à jour de positions
    /// </summary>
    public async Task UnsubscribeFromUpdates(string? province = null)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"vehicles-{province ?? "all"}");
        _logger.LogDebug("Client unsubscribed from vehicles updates: {Province}", province ?? "all");
    }
}