using System.Text;
using FunkoAppi01.Config;
using FunkoAppi01.Repository;
using FunkoAppi01.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FunkoAppi01.Infraestructure;

/// <summary>
/// Registra la autenticación JWT (enfoque manual: sin ASP.NET Core Identity).
/// </summary>
public static class AuthenticationConfig
{
    public static IServiceCollection AddAuthenticationJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtConfig.SectionName);
        services.Configure<JwtConfig>(jwtSection);

        var jwtConfig = new JwtConfig();
        jwtSection.Bind(jwtConfig);

        if (string.IsNullOrWhiteSpace(jwtConfig.Secret))
            throw new InvalidOperationException("Jwt:Secret no configurado");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Sin mapeo inbound: los claims conservan sus nombres originales
                // ("sub", "username", "role"), que es lo que lee AuthController.GetCurrentUser
                // y lo que RoleClaimType necesita para que [Authorize(Roles = ...)] funcione.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();

        services.AddScoped<IRepositoryUser, RepositoryUser>();
        services.AddScoped<JwtService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
