using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Service;

public interface IServiceCategoria
{
    // Devuelve todas las categorías.
    Task<IEnumerable<CategoriaDto>> GetAllAsync();

    // Devuelve la categoría o un error si no existe.
    Task<Result<CategoriaDto, DomainError>> GetByIdAsync(long id);

    // Valida y crea una categoría.
    Task<Result<CategoriaDto, DomainError>> AddAsync(
        CreateCategoriaDto request);

    // Valida y actualiza una categoría.
    Task<Result<CategoriaDto, DomainError>> UpdateAsync(
        long id,
        UpdateCategoriaDto request);

    // Borra una categoría si existe y no tiene Funkos asociados.
    Task<Result<bool, DomainError>> DeleteAsync(long id);
}