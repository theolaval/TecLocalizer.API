namespace TecLocalizer.API.DTOs;

public class VehicleDto
{
    public string VehicleId { get; set; } = string.Empty;
    public string RouteShortName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
}