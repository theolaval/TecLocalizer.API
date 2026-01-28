using Microsoft.EntityFrameworkCore;
using TecLocalizer.DL.Models;

namespace TecLocalizer.DAL.Repositories;

public class TecDbContext : DbContext
{
    public DbSet<Stop> Stops { get; set; }
    public DbSet<DL.Models.Route> Routes { get; set; } 
    public DbSet<VehiclePosition> VehiclePositions { get; set; }

    public TecDbContext(DbContextOptions<TecDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stop>().HasKey(s => s.StopId);
        modelBuilder.Entity<DL.Models.Route>().HasKey(r => r.RouteId);
        modelBuilder.Entity<VehiclePosition>().HasKey(v => v.VehicleId);
    }
}
