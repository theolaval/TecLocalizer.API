using TecLocalizer.BLL.DTOs;
using TecLocalizer.BLL.Services.Interfaces;
using TecLocalizer.DL.Enums;
using TecLocalizer.DL.Models;
using Route = TecLocalizer.DL.Models.Route;

namespace TecLocalizer.BLL.Services;

/// <summary>
/// Service GTFS pour récupérer les arrêts et routes
/// Implémentation avec données mockées pour le prototype
/// </summary>
public class GtfsService : IGtfsService
{
    private List<Stop> _stops = new();
    private List<Route> _routes = new();
    private DateTime _lastUpdateTime = DateTime.UtcNow;
    private readonly ILogger<GtfsService> _logger;

    public DateTime LastUpdateTime => _lastUpdateTime;

    public GtfsService(ILogger<GtfsService> logger)
    {
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // TODO: Charger depuis API GTFS réelle
            // Pour maintenant, utiliser données mockées
            _stops = GetMockStops();
            _routes = GetMockRoutes();
            _lastUpdateTime = DateTime.UtcNow;
            _logger.LogInformation("GTFS Service initialized with {StopCount} stops", _stops.Count);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing GTFS Service");
            throw;
        }
    }

    public Task<List<StopDto>> GetAllStopsAsync(Province? province = null)
    {
        var stops = _stops.AsEnumerable();
        
        if (province.HasValue && province.Value != Province.All)
        {
            stops = stops.Where(s => s.Province == province);
        }

        var dtos = stops.Select(s => new StopDto
        {
            StopId = s.StopId,
            Name = s.Name,
            Code = s.Code,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            Province = s.Province.ToString()
        }).ToList();

        return Task.FromResult(dtos);
    }

    public Task<List<VehicleDto>> GetVehiclePositionsAsync(Province? province = null)
    {
        // Générées dynamiquement avec petites variations
        var vehicles = GenerateMockVehiclePositions();
        
        if (province.HasValue && province.Value != Province.All)
        {
            vehicles = vehicles.Where(v => v.Province == province.ToString()).ToList();
        }

        return Task.FromResult(vehicles);
    }

    private List<Stop> GetMockStops()
    {
        return new List<Stop>
        {
            // Liège
            new Stop { StopId = "LGE001", Name = "Liège Central", Code = "LGEC", Latitude = 50.6321, Longitude = 5.5672, Province = Province.Liege },
            new Stop { StopId = "LGE002", Name = "Liège Guillemins", Code = "LGEG", Latitude = 50.6312, Longitude = 5.5702, Province = Province.Liege },
            new Stop { StopId = "LGE003", Name = "Seraing Centre", Code = "SER001", Latitude = 50.6142, Longitude = 5.4825, Province = Province.Liege },
            new Stop { StopId = "LGE004", Name = "Herstal Gare", Code = "HER001", Latitude = 50.6089, Longitude = 5.5089, Province = Province.Liege },
            new Stop { StopId = "LGE005", Name = "Athénée", Code = "ATH001", Latitude = 50.6276, Longitude = 5.5492, Province = Province.Liege },
            new Stop { StopId = "LGE006", Name = "Theux Centre", Code = "THX001", Latitude = 50.5892, Longitude = 5.5765, Province = Province.Liege },

            // Namur
            new Stop { StopId = "NAM001", Name = "Namur Gare", Code = "NAMG", Latitude = 50.4668, Longitude = 4.8714, Province = Province.Namur },
            new Stop { StopId = "NAM002", Name = "Namur Centre", Code = "NAMC", Latitude = 50.4672, Longitude = 4.8690, Province = Province.Namur },
            new Stop { StopId = "NAM003", Name = "Dinant Gare", Code = "DIN001", Latitude = 50.2618, Longitude = 4.9022, Province = Province.Namur },
            new Stop { StopId = "NAM004", Name = "Ciney Centre", Code = "CIN001", Latitude = 50.2911, Longitude = 4.6124, Province = Province.Namur },
            new Stop { StopId = "NAM005", Name = "Gembloux Gare", Code = "GEM001", Latitude = 50.5694, Longitude = 4.7164, Province = Province.Namur },

            // Hainaut
            new Stop { StopId = "HAI001", Name = "Mons Gare", Code = "MONSG", Latitude = 50.4539, Longitude = 3.9546, Province = Province.Hainaut },
            new Stop { StopId = "HAI002", Name = "Mons Centre", Code = "MONSC", Latitude = 50.4548, Longitude = 3.9561, Province = Province.Hainaut },
            new Stop { StopId = "HAI003", Name = "Charleroi Gare", Code = "CHARG", Latitude = 50.4077, Longitude = 4.4325, Province = Province.Hainaut },
            new Stop { StopId = "HAI004", Name = "Tournai Gare", Code = "TOURG", Latitude = 50.6054, Longitude = 3.3864, Province = Province.Hainaut },

            // Brabant Wallon
            new Stop { StopId = "BRA001", Name = "Wavre Centre", Code = "WAV001", Latitude = 50.7197, Longitude = 4.6183, Province = Province.BrabantWallon },
            new Stop { StopId = "BRA002", Name = "Ottignies Gare", Code = "OTT001", Latitude = 50.6567, Longitude = 4.5675, Province = Province.BrabantWallon },
            new Stop { StopId = "BRA003", Name = "Nivelles Centre", Code = "NIV001", Latitude = 50.5911, Longitude = 4.3370, Province = Province.BrabantWallon },

            // Luxembourg
            new Stop { StopId = "LUX001", Name = "Arlon Gare", Code = "ARL001", Latitude = 49.6820, Longitude = 5.8016, Province = Province.Luxembourg },
            new Stop { StopId = "LUX002", Name = "Virton Centre", Code = "VIR001", Latitude = 49.5807, Longitude = 5.4337, Province = Province.Luxembourg },
            new Stop { StopId = "LUX003", Name = "Bastogne Gare", Code = "BAS001", Latitude = 50.0080, Longitude = 5.7270, Province = Province.Luxembourg },
        };
    }

    private List<Route> GetMockRoutes()
    {
        return new List<Route>
        {
            new Route { RouteId = "1", ShortName = "1", LongName = "Liège - Seraing", Province = Province.Liege },
            new Route { RouteId = "2", ShortName = "2", LongName = "Liège - Herstal", Province = Province.Liege },
            new Route { RouteId = "4", ShortName = "4", LongName = "Namur - Gembloux", Province = Province.Namur },
            new Route { RouteId = "5", ShortName = "5", LongName = "Mons - Charleroi", Province = Province.Hainaut },
            new Route { RouteId = "120", ShortName = "120", LongName = "Wavre - Ottignies", Province = Province.BrabantWallon },
        };
    }

    private List<VehicleDto> GenerateMockVehiclePositions()
    {
        var random = new Random(DateTime.Now.Millisecond);
        var vehicles = new List<VehicleDto>();

        var provincePositions = new Dictionary<Province, (double baseLat, double baseLng)>
        {
            { Province.Liege, (50.6321, 5.5672) },
            { Province.Namur, (50.4668, 4.8714) },
            { Province.Hainaut, (50.4539, 3.9546) },
            { Province.BrabantWallon, (50.7197, 4.6183) },
            { Province.Luxembourg, (49.6820, 5.8016) }
        };

        var routes = new[] { "1", "2", "4", "5", "120", "201", "202", "301" };

        foreach (var province in provincePositions.Keys)
        {
            var (baseLat, baseLng) = provincePositions[province];
            
            for (int i = 0; i < 3; i++)
            {
                vehicles.Add(new VehicleDto
                {
                    VehicleId = $"V-{province}-{i}",
                    RouteShortName = routes[random.Next(routes.Length)],
                    Latitude = baseLat + (random.NextDouble() - 0.5) * 0.1,
                    Longitude = baseLng + (random.NextDouble() - 0.5) * 0.1,
                    Speed = random.NextDouble() * 50,
                    DelayMinutes = random.Next(-5, 15),
                    Province = province.ToString(),
                    UpdatedAt = DateTime.UtcNow.AddSeconds(-random.Next(0, 30))
                });
            }
        }

        return vehicles;
    }
}
