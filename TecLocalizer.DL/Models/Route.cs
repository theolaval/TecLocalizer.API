using TecLocalizer.DL.Enums;

namespace TecLocalizer.DL.Models;

public class Route
{
    public string RouteId { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string LongName { get; set; } = string.Empty;
    public string? RouteType { get; set; }
    public string? AgencyId { get; set; }
    public Province Province { get; set; } = Province.All;
}