using FunkoAppi01.Config;
using FunkoAppi01.Entity;
using Microsoft.EntityFrameworkCore;

namespace FunkoAppi01.Infraestructure;

public static class PersistenceConfig
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfig = new DatabaseConfig();
        configuration.GetSection(DatabaseConfig.SectionName).Bind(databaseConfig);

        services.AddDbContext<AppDbContext>(options =>
        {
            if (databaseConfig.Provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
                options.UseNpgsql(databaseConfig.ConnectionString);
            else
                options.UseSqlite(databaseConfig.ConnectionString);
        });

        return services;
    }
}
