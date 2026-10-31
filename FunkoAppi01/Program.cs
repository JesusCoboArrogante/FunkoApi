using System.Text.Json;
using FunkoAppi01.Entity;
using FunkoAppi01.Infraestructure;
using FunkoAppi01.Middleware;
using FunkoAppi01.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega aquí el token, con el prefijo \"Bearer \" (p. ej. \"Bearer eyJ...\")"
    });
});

builder.Services
    .AddPersistence(builder.Configuration)
    .AddRepositories()
    .AddValidators()
    .AddServices()
    .AddAuthenticationJwt(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // El proveedor InMemory (tests de integracion con WebApplicationFactory) no
    // soporta migraciones; solo SQLite/PostgreSQL (relacionales) las necesitan.
    if (db.Database.IsRelational())
        await db.Database.MigrateAsync();
    else
        await db.Database.EnsureCreatedAsync();

    await UserSeeder.SeedAsync(db);
}

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// El orden importa: primero quien eres (Authentication), luego que puedes hacer (Authorization).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Visible para WebApplicationFactory<Program> en los tests de integracion
// (Program.cs usa Top Level Statements, que generan una clase Program implicita).
public partial class Program;