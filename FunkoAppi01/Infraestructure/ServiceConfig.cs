using FunkoAppi01.Service;

namespace FunkoAppi01.Infraestructure;

public static class ServiceConfig
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        {
            services.AddScoped<IServiceFunko, ServiceFunko>();
            return services;
        }
    }
}


