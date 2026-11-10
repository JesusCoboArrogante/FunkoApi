using System.ComponentModel.DataAnnotations;

namespace FunkoAppi01.GraphQL.Input;

public record CreateProveedorInput()
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre {get; init; } = string.Empty;

    [RegularExpression(@"^[67][0-9]{8}$", ErrorMessage = "El telefono solo puede tener 9 numeros")]
    public int Telefono { get; set; }
}