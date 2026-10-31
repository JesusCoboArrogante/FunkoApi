using System.ComponentModel.DataAnnotations;

namespace FunkoAppi01.Dto;

/// <summary>
/// DTO con las credenciales de inicio de sesión.
/// </summary>
/// <param name="Email">Correo electrónico.</param>
/// <param name="Password">Contraseña en texto plano (solo viaja en la petición).</param>
public record LoginDto(
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    string Email,

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    string Password
);
