using System.ComponentModel.DataAnnotations;

namespace FunkoAppi01.Dto;

public record CreateProveedorDto
    (
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        string Nombre,
        
        [RegularExpression(@"^[67][0-9]{8}$", ErrorMessage = "El telefono solo puede tener 9 numeros")]
        int Telefono
    );