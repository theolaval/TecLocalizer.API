namespace TecLocalizer.BLL.DTOs;

public class VehicleDto
{
    public string VehicleId { get; set; } = string.Empty;
    public string RouteShortName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public int DelayMinutes { get; set; }
    public string Province { get; set; } = "All";
    public DateTime UpdatedAt { get; set; }
}
