namespace TecLocalizer.DL.Models;
public class VehiclePosition
{
    public string VehicleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public DateTime UpdatedAt { get; set; }
}