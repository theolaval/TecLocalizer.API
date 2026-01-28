namespace TecLocalizer.API.DTOs;

public class StopDto
{
    public string StopId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}