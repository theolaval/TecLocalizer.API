using TecLocalizer.BLL.DTOs;
using TecLocalizer.BLL.Services.Interfaces;
using TecLocalizer.DL.Enums;

namespace TecLocalizer.BLL.Services;

public class VehiclePositionService : IVehiclePositionService, IHostedService
{
    private readonly IGtfsService _gtfsService;
    private readonly ILogger<VehiclePositionService> _logger;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _pollingTask;
    private DateTime _lastUpdateTime = DateTime.UtcNow;
    private List<VehicleDto> _cachedVehicles = new();
    private readonly object _cacheLock = new();
    
    private const int PollingIntervalSeconds = 30;

    public DateTime LastUpdateTime => _lastUpdateTime;

    public VehiclePositionService(IGtfsService gtfsService, ILogger<VehiclePositionService> logger)
    {
        _gtfsService = gtfsService;
        _logger = logger;
    }

    public Task<List<VehicleDto>> GetCurrentPositionsAsync(Province? province = null)
    {
        lock (_cacheLock)
        {
            var result = _cachedVehicles.AsEnumerable();
            
            if (province.HasValue && province.Value != Province.All)
            {
                result = result.Where(v => v.Province == province.ToString());
            }

            return Task.FromResult(result.ToList());
        }
    }

    public Task StartPollingAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource != null)
        {
            _logger.LogWarning("Vehicle position polling already started");
            return Task.CompletedTask;
        }

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _pollingTask = PollVehiclePositionsAsync(_cancellationTokenSource.Token);
        _logger.LogInformation("Vehicle position polling started (interval: {IntervalSeconds}s)", PollingIntervalSeconds);
        
        return Task.CompletedTask;
    }

    public Task StopPollingAsync()
    {
        if (_cancellationTokenSource == null)
        {
            return Task.CompletedTask;
        }

        _cancellationTokenSource.Cancel();
        _logger.LogInformation("Vehicle position polling stopped");
        
        return Task.CompletedTask;
    }

    private async Task PollVehiclePositionsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var positions = await _gtfsService.GetVehiclePositionsAsync();
                
                lock (_cacheLock)
                {
                    _cachedVehicles = positions;
                    _lastUpdateTime = DateTime.UtcNow;
                }

                _logger.LogDebug("Vehicle positions updated: {Count} vehicles", positions.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling vehicle positions");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(PollingIntervalSeconds), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _gtfsService.InitializeAsync();
        await StartPollingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopPollingAsync();
        
        if (_pollingTask != null)
        {
            try
            {
                await _pollingTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}
