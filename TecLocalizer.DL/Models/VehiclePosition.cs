using TecLocalizer.DL.Enums;

namespace TecLocalizer.DL.Models;

public class VehiclePosition
{
    public string VehicleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteShortName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public int DelayMinutes { get; set; }
    public Province Province { get; set; } = Province.All;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}