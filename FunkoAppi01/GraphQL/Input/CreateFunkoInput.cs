using System.ComponentModel.DataAnnotations;

namespace FunkoAppi01.GraphQL.Input;

public record CreateFunkoInput()
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
    public string Name { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
    public decimal Precio { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock  { get; init; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public long CategoryId { get; init; } 
}