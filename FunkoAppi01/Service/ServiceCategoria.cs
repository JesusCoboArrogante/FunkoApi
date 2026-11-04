using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;
using FunkoAppi01.Mappers;
using FunkoAppi01.Repository;
using FunkoAppi01.Validators;

namespace FunkoAppi01.Service;

public class ServiceCategoria (
    IRepositoryCategoria repository,
    ILogger <ServiceCategoria> logger,
    ICategoriaValidator validator) : IServiceCategoria
{
    public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
    {
        var categoria = await repository.GetAllAsync();
        return categoria.Select(c => c.ToDto());
    }

    public async Task<Result<CategoriaDto, DomainError>> GetByIdAsync(long id)
    {
        var categoria = await repository.GetByIdAsync(id);
        if (categoria == null)
        {
            return Result.Failure<CategoriaDto, DomainError>(
                new DomainError.NotFound(id, "Categoria"));
        }
        
        return Result.Success<CategoriaDto, DomainError>(categoria.ToDto());
    }

    public async Task<Result<CategoriaDto, DomainError>> AddAsync( CreateCategoriaDto request)
    {
        var validacion = validator.ValidateCreate(request);

        if (validacion.IsFailure)
        {
            return Result.Failure<CategoriaDto, DomainError>(validacion.Error);
        }

        var categoria = request.ToCategoria();
        categoria.Nombre = categoria.Nombre.Trim();

        if (await repository.ExistsByNameAsync(categoria.Nombre))
        {
            return Result.Failure<CategoriaDto, DomainError>(
                new DomainError.AlreadyExists(
                    categoria.Nombre, "Categoría"));
        }

        var creada = await repository.AddAsync(categoria);

        logger.LogInformation(
            "Categoría creada: {Id} - {Nombre}",
            creada.Id,
            creada.Nombre);

        return Result.Success<CategoriaDto, DomainError>(
            creada.ToDto());
    }

    public async Task<Result<CategoriaDto, DomainError>> UpdateAsync(long id, UpdateCategoriaDto request)
    {
        var validacion = validator.ValidateUpdate(request);

        if (validacion.IsFailure)
        {
            return Result.Failure<CategoriaDto, DomainError>(
                validacion.Error);
        }

        var existente = await repository.GetByIdAsync(id);

        if (existente is null)
        {
            return Result.Failure<CategoriaDto, DomainError>(
                new DomainError.NotFound(id, "Categoría"));
        }

        var categoria = request.ToCategoria();
        categoria.Nombre = categoria.Nombre.Trim();

        if (await repository.ExistsByNameAsync(categoria.Nombre, id))
        {
            return Result.Failure<CategoriaDto, DomainError>(
                new DomainError.AlreadyExists(
                    categoria.Nombre, "Categoría"));
        }

        var actualizada = await repository.UpdateAsync(id, categoria);

        if (actualizada is null)
        {
            return Result.Failure<CategoriaDto, DomainError>(
                new DomainError.NotFound(id, "Categoría"));
        }

        logger.LogInformation(
            "Categoría actualizada: {Id} - {Nombre}",
            actualizada.Id,
            actualizada.Nombre);

        return Result.Success<CategoriaDto, DomainError>(
            actualizada.ToDto());

    }

    public async Task<Result<bool, DomainError>> DeleteAsync(long id)
    {
        var categoria = await repository.GetByIdAsync(id);

        if (categoria is null)
        {
            return Result.Failure<bool, DomainError>(
                new DomainError.NotFound(id, "Categoría"));
        }

        if (await repository.HasFunkoAsync(id))
        {
            return Result.Failure<bool, DomainError>(
                new DomainError.Conflict(
                    $"No se puede borrar la categoría {id} porque tiene Funkos asociados"));
        }

        var eliminada = await repository.SoftDeleteAsync(id);

        if (!eliminada)
        {
            return Result.Failure<bool, DomainError>(
                new DomainError.NotFound(id, "Categoría"));
        }

        logger.LogInformation(
            "Categoría eliminada: {Id}", id);

        return Result.Success<bool, DomainError>(true);
    }
}
