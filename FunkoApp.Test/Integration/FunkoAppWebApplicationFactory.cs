using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FunkoApp.Test.Integration;

/// <summary>
/// Levanta la API completa en memoria para los tests de integración.
/// En vez de manipular los servicios de EF Core ya registrados (frágil: dos
/// proveedores de base de datos no pueden convivir en el mismo ServiceProvider),
/// se le indica a la propia configuración que use el proveedor "InMemory" con un
/// nombre de base de datos único por instancia, igual que decide entre SQLite y
/// PostgreSQL en producción. Se usa UseSetting (no ConfigureAppConfiguration):
/// con Top Level Statements, "builder.Configuration" de Program.cs ya está
/// resuelto antes de que ConfigureAppConfiguration pudiera añadir nada.
/// </summary>
public class FunkoAppWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"FunkoAppTests_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("DatabaseSettings:Provider", "InMemory");
        builder.UseSetting("DatabaseSettings:ConnectionString", _databaseName);
    }
}
