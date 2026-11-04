using FunkoAppi01.Dto;
using FunkoAppi01.Models;

namespace FunkoAppi01.Mappers;

public static class CategoriaMapper
{
    public static CategoriaDto ToDto(this Categoria categoria)
    {
        return new CategoriaDto(
            categoria.Id,
            categoria.Nombre
        );
    }

    public static Categoria ToCategoria(this CreateCategoriaDto dto)
    {
        return new Categoria
        {
            Nombre = dto.Nombre
        };
    }

    public static Categoria ToCategoria(this UpdateCategoriaDto dto)
    {
        return new Categoria
        {
            Nombre = dto.Nombre
        };
    }

}