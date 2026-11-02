namespace FunkoAppi01.Dto;

/// <summary>
/// DTO con el resultado de un login o signup correcto.
/// </summary>
/// <param name="AccessToken">Token JWT firmado.</param>
/// <param name="TokenType">Tipo de token (siempre "Bearer").</param>
/// <param name="ExpiresIn">Segundos de validez del token.</param>
/// <param name="UserId">Identificador del usuario.</param>
/// <param name="Username">Nombre de usuario.</param>
/// <param name="Role">Rol del usuario (ADMIN o USER).</param>
public record AuthResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    long UserId,
    string Username,
    string Role
);
