using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;
using FunkoAppi01.Mappers;
using FunkoAppi01.Repository;
using FunkoAppi01.Validators;

namespace FunkoAppi01.Service;

public class ServiceProveedor (
    IRepositoryProveedor repository,
    ILogger<ServiceProveedor> logger,
    IProveedoValidator validator
        ) : IServiceProveedor
{
    public async Task<IEnumerable<ProveedorDto>> GetAllAsync()
    {
        var proveedor  = await repository.GetAllAsync();
        return proveedor.Select(p => p.ToDto());

    }

    public async Task<Result<ProveedorDto, DomainError>> GetByIdAsync(long id)
    {
        var proveedor = await repository.GetByIdAsync(id);
        if (proveedor == null)
        {
            return Result.Failure<ProveedorDto, DomainError>(
                new DomainError.NotFound(id, "Proveedor")
            );
        }
        
        return Result.Success<ProveedorDto, DomainError>(proveedor.ToDto());
    }

    public async Task<Result<ProveedorDto, DomainError>> AddAsync(CreateProveedorDto request)
    {
        var validacion = validator.ValidateCreate(request);
        if (validacion.IsFailure)
        {
            return Result.Failure<ProveedorDto, DomainError>(validacion.Error);
        }

        var proveedor = request.ToProveedor();
        proveedor.Nombre = proveedor.Nombre.Trim();
        proveedor.Telefono = proveedor.Telefono;

        if (await repository.ExistsByNameAsync(proveedor.Nombre))
        {
            return Result.Failure<ProveedorDto, DomainError>(
                new DomainError.AlreadyExists(
                    proveedor.Nombre, "Proveedor")
            );
        }
        
        var creada = await repository.AddAsync(proveedor);

        logger.LogInformation(
            "Proveedor creado: {Id} - {Nombre} - {Telefono}",
            creada.Id,
            creada.Nombre,
            creada.Telefono
        );
        
        return Result.Success<ProveedorDto, DomainError>(creada.ToDto());
    }

    public async Task<Result<ProveedorDto, DomainError>> UpdateAsync(long id, UpdateProveedorDto request)
    {
        var validacion = validator.ValidateUpdate(request);
        if (validacion.IsFailure)
        {
            return Result.Failure<ProveedorDto, DomainError>(validacion.Error);
        }
        var existe = await repository.GetByIdAsync(id);

        if (existe is null)
        {
            return Result.Failure<ProveedorDto, DomainError>(
                new DomainError.NotFound(id, "Proveedor")
            );
        }
        
        var proveedor = request.ToProveedor();
        proveedor.Nombre = proveedor.Nombre.Trim();
        proveedor.Telefono = proveedor.Telefono;

        if (await repository.ExistsByNameAsync(proveedor.Nombre, id))
        {
            return Result.Failure<ProveedorDto, DomainError>(
                new DomainError.NotFound(id, "Proveedor"));
        }

        var actualizada = await repository.UpdateAsync(id, proveedor);

        if (actualizada is null)
        {
            return Result.Failure<ProveedorDto, DomainError>(
                new DomainError.NotFound(id, "Proveedor")
            );
        }
        
        logger.LogInformation(
            "Proveedor actualizado: {Id} - {Nombre} - {Telefono}",
            actualizada.Id,
            actualizada.Nombre,
            actualizada.Telefono
            );
        
        return  Result.Success<ProveedorDto, DomainError>(actualizada.ToDto());
    }

    public async Task<Result<bool, DomainError>> DeleteAsync(long id)
    {
        var proveedor = await repository.GetByIdAsync(id);
        if (proveedor is null)
        {
            return Result.Failure<bool, DomainError>(
                new DomainError.NotFound(id, "Proveedor")
            );
        }

        if (await repository.HasFunkoAsync(id))
        {
            return Result.Failure<bool, DomainError>(
                new DomainError.Conflict(
                    $"No se puede borrar el proveedor {id} por que tienen Funkos asociados")
            );
        }

        var eliminada = await repository.SoftDeleteAsync(id);

        if (!eliminada)
        {
            return Result.Failure<bool, DomainError>(
                 new DomainError.NotFound(id, "Proveedor")
                );
        }
        
        logger.LogInformation(
            "Proveedor eliminado: {id}", id);
        
        return Result.Success<bool, DomainError>(true);
    }
}