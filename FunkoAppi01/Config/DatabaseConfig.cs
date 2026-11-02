namespace FunkoAppi01.Config;

/// <summary>
/// Configuración de la base de datos local.
/// </summary>
public class DatabaseConfig
{
    /// <summary>Nombre de la sección en appsettings.json.</summary>
    public const string SectionName = "DatabaseSettings";

    /// <summary>Proveedor: "Sqlite" (perfil Dev) o "PostgreSQL" (perfil Prod).</summary>
    public string Provider { get; set; } = "Sqlite";

    /// <summary>Cadena de conexión a la base de datos.</summary>
    public string ConnectionString { get; set; } = "Data Source=funkos.db";
}
