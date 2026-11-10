namespace FunkoAppi01.Dto;
/// <summary>
/// DTO con los datos de una Categoria que el servicio devuelve hacia fuera.
/// </summary>
/// <param name="Id">Identificador.</param>
/// <param name="Nombre">Nombre de la categoria.</param>
public record CategoriaDto(
    long Id,
    string Nombre
);
    
