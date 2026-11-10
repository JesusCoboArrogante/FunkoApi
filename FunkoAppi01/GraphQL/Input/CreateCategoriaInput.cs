using System.ComponentModel.DataAnnotations;

namespace FunkoAppi01.GraphQL.Input;

public record CreateCategoriaInput
{
    [Required(ErrorMessage = "Nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Nombre { get; init; } = string.Empty;
}