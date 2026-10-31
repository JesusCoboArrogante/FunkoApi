using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;
using FunkoAppi01.Mappers;
using FunkoAppi01.Repository;
using FunkoAppi01.Validators;
using Microsoft.Extensions.Logging;

namespace FunkoAppi01.Service;

/// <summary>
/// Orquesta las reglas de negocio de Funko entre el validador y el repositorio.
/// </summary>
public class ServiceFunko(
    IRepositoryFunko repository,
    IFunkoValidator validator,
    ILogger<ServiceFunko> logger) : IServiceFunko
{
    public IEnumerable<FunkoDto> GetAll() =>
        repository.GetAll().Select(f => f.ToDto());

    public Result<FunkoDto, DomainError> GetById(long id)
    {
        if (repository.GetById(id) is not { } funko)
            return Result.Failure<FunkoDto, DomainError>(FunkoErrors.NotFound(id));

        return Result.Success<FunkoDto, DomainError>(funko.ToDto());
    }

    public Result<FunkoDto, DomainError> Add(CreateFunkoDto request)
    {
        var validacion = validator.ValidateCreate(request);
        if (validacion.IsFailure)
            return Result.Failure<FunkoDto, DomainError>(validacion.Error);

        if (repository.ExistsByName(request.Nombre))
        {
            logger.LogWarning("Intento de crear un Funko con nombre duplicado: {Nombre}", request.Nombre);
            return Result.Failure<FunkoDto, DomainError>(FunkoErrors.AlreadyExists(request.Nombre));
        }

        var creado = repository.Add(request.ToFunko());
        logger.LogInformation("Funko creado: {Id} - {Nombre}", creado.Id, creado.Nombre);
        return Result.Success<FunkoDto, DomainError>(creado.ToDto());
    }

    public Result<FunkoDto, DomainError> Update(long id, UpdateFunkoDto request)
    {
        var validacion = validator.ValidateUpdate(request);
        if (validacion.IsFailure)
            return Result.Failure<FunkoDto, DomainError>(validacion.Error);

        if (repository.ExistsByName(request.Nombre, id))
            return Result.Failure<FunkoDto, DomainError>(FunkoErrors.AlreadyExists(request.Nombre));

        if (repository.Update(id, request.ToFunko()) is not { } actualizado)
        {
            logger.LogWarning("Intento de actualizar un Funko inexistente: {Id}", id);
            return Result.Failure<FunkoDto, DomainError>(FunkoErrors.NotFound(id));
        }

        logger.LogInformation("Funko actualizado: {Id} - {Nombre}", actualizado.Id, actualizado.Nombre);
        return Result.Success<FunkoDto, DomainError>(actualizado.ToDto());
    }

    public Result<FunkoDto, DomainError> UpdatePrice(long id, decimal precio)
    {
        if (precio < 0)
            return Result.Failure<FunkoDto, DomainError>(
                FunkoErrors.Validation(["El precio no puede ser negativo"]));

        if (repository.UpdatePrecio(id, precio) is not { } actualizado)
            return Result.Failure<FunkoDto, DomainError>(FunkoErrors.NotFound(id));

        return Result.Success<FunkoDto, DomainError>(actualizado.ToDto());
    }

    public Result<bool, DomainError> Delete(long id)
    {
        if (!repository.SoftDelete(id))
        {
            logger.LogWarning("Intento de borrar un Funko inexistente: {Id}", id);
            return Result.Failure<bool, DomainError>(FunkoErrors.NotFound(id));
        }

        logger.LogInformation("Funko borrado (logico): {Id}", id);
        return Result.Success<bool, DomainError>(true);
    }

    public IEnumerable<FunkoDto> GetByName(string termino) =>
        repository.GetByName(termino).Select(f => f.ToDto());
}
