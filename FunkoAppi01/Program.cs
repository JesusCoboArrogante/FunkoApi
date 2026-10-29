using System.Text.Json;
using FunkoAppi01.Infraestructure;
using FunkoAppi01.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services
    .AddRepositories()
    .AddServices();

var app = builder.Build();

app.MapControllers();

app.Run();