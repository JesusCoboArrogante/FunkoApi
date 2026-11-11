using FunkoAppi01.Validators;

namespace FunkoAppi01.Infraestructure;

public static class ValidatorsConfig
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddSingleton<IFunkoValidator, FunkoValidator>();
        services.AddSingleton<ICategoriaValidator, CategoriaValidator>();
        services.AddSingleton<IProveedoValidator, ProveedorValidator>();
        return services;
    }
}
