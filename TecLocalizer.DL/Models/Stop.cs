using TecLocalizer.DL.Enums;

namespace TecLocalizer.DL.Models;

public class Stop
{
    public string StopId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Province Province { get; set; } = Province.All;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}