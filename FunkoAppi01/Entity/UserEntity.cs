namespace FunkoAppi01.Entity;

/// <summary>
/// Entidad de persistencia para el usuario con autenticación JWT.
/// La configuración (tabla, columnas, índices) está en AppDbContext con Fluent API.
/// </summary>
public class UserEntity
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.User;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

/// <summary>
/// Constantes para los roles de usuario.
/// </summary>
public static class UserRoles
{
    public const string Admin = "ADMIN";
    public const string User = "USER";
}
