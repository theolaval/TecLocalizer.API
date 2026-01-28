using Microsoft.EntityFrameworkCore;
using TecLocalizer.DAL.Repositories;
using TecLocalizer.DAL.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

#region Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

#region Database
builder.Services.AddDbContext<TecLocalizer.DAL.Repositories.TecDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Repositories
builder.Services.AddScoped<TecLocalizer.DAL.Repositories.Interfaces.IVehicleRepository, TecLocalizer.DAL.Repositories.VehicleRepository>();
#endregion
#endregion

var app = builder.Build();

#region Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<TecLocalizer.API.Hubs.VehicleHub>("/hubs/vehicles");

#endregion

app.Run();