using System.ComponentModel.DataAnnotations;

namespace FunkoAppi01.Dto;

/// <summary>
/// DTO con los datos para registrar un usuario nuevo.
/// </summary>
/// <param name="Username">Nombre de usuario.</param>
/// <param name="Email">Correo electrónico.</param>
/// <param name="Password">Contraseña en texto plano (se hashea con BCrypt antes de guardarla).</param>
public record SignupDto(
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre de usuario no puede superar los 100 caracteres")]
    string Username,

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
    string Email,

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    string Password
);
