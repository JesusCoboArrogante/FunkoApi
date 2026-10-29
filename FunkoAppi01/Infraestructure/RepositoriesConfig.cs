
using FunkoAppi01.Repository;

namespace FunkoAppi01.Infraestructure;

public static class RepositoriesConfig
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        
        services.AddSingleton<IRepositoryFunko, RepositoryFunko>();
        return services;
    }
}