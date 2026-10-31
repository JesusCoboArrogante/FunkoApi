namespace FunkoAppi01.Config;

/// <summary>
/// Configuración de la autenticación JWT.
/// </summary>
public class JwtConfig
{
    /// <summary>Nombre de la sección en appsettings.json.</summary>
    public const string SectionName = "Jwt";

    /// <summary>Clave secreta para firmar los tokens.</summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>Emisor del token.</summary>
    public string Issuer { get; set; } = "FunkoApp";

    /// <summary>Audiencia del token.</summary>
    public string Audience { get; set; } = "FunkoAppClients";

    /// <summary>Minutos de validez del access token.</summary>
    public int ExpirationMinutes { get; set; } = 15;
}
