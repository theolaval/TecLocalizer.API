using Microsoft.EntityFrameworkCore;
using TecLocalizer.BLL.Services;
using TecLocalizer.BLL.Services.Interfaces;
using TecLocalizer.DAL.Repositories;
using TecLocalizer.DAL.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// CORS configuration for React localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:5210",
                "https://localhost:3001",
                "https://localhost:5173",
                "https://localhost:5174",
                "https://localhost:5001"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Business Logic Layer Services
builder.Services.AddSingleton<IGtfsService, GtfsService>();
builder.Services.AddSingleton<IVehiclePositionService, VehiclePositionService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<IVehiclePositionService>() as VehiclePositionService
    ?? throw new InvalidOperationException("VehiclePositionService not found"));

// Database
builder.Services.AddDbContext<TecDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? ""));

// Repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

var app = builder.Build();

// Force HTTP port to 5000 in development
if (app.Environment.IsDevelopment())
{
    app.Urls.Clear();
    app.Urls.Add("http://localhost:5000");
    app.Urls.Add("https://localhost:5001");
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.MapControllers();
app.MapHub<TecLocalizer.API.Hubs.VehicleHub>("/hubs/vehicles");

app.Run();