namespace FunkoAppi01.Errors;

/// <summary>
/// Errores de dominio. Cada tipo de error construye su propio mensaje.
/// </summary>
/// <param name="Message">Mensaje.</param>
public abstract record DomainError(string Message)
{
    /// <summary>Cuando no existe ningún Funko con ese identificador.</summary>
    /// <param name="Id">Identificador buscado.</param>
    public sealed record NotFound(long Id)
        : DomainError($"No se ha encontrado ningún Funko con el identificador: {Id}");

    /// <summary>Los datos no cumplen con el validador.</summary>
    /// <param name="Errors">Lista de errores detectados.</param>
    public sealed record Validation(IEnumerable<string> Errors)
        : DomainError(string.Join(", ", Errors));

    /// <summary>Ya existe un Funko activo con ese nombre.</summary>
    /// <param name="Nombre">Nombre duplicado.</param>
    public sealed record AlreadyExists(string Nombre)
        : DomainError($"Ya existe un Funko con el nombre: {Nombre}");

    /// <summary>Credenciales inválidas o token no válido.</summary>
    /// <param name="Reason">Motivo.</param>
    public sealed record Unauthorized(string Reason) : DomainError(Reason);

    /// <summary>Conflicto genérico (p. ej. un email ya registrado).</summary>
    /// <param name="Reason">Motivo.</param>
    public sealed record Conflict(string Reason) : DomainError(Reason);
}

/// <summary>
/// Factoría para crear errores de dominio de Funko.
/// </summary>
public static class FunkoErrors
{
    /// <summary>Funko no encontrado.</summary>
    public static DomainError NotFound(long id) => new DomainError.NotFound(id);

    /// <summary>Validación con la lista de errores.</summary>
    public static DomainError Validation(IEnumerable<string> errors) => new DomainError.Validation(errors);

    /// <summary>Nombre ya existente entre los Funkos activos.</summary>
    public static DomainError AlreadyExists(string nombre) => new DomainError.AlreadyExists(nombre);
}

/// <summary>
/// Factoría para crear errores de dominio de autenticación.
/// </summary>
public static class AuthErrors
{
    /// <summary>Credenciales inválidas.</summary>
    public static DomainError Unauthorized(string reason) => new DomainError.Unauthorized(reason);

    /// <summary>Email ya registrado.</summary>
    public static DomainError Conflict(string reason) => new DomainError.Conflict(reason);
}
