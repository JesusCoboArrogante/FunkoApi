using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Validators;

/// <summary>
/// Validador de dominio de las peticiones de Funko.
/// </summary>
public interface IFunkoValidator
{
    /// <summary>Comprueba que los datos de creación son correctos.</summary>
    /// <param name="request">Datos recibidos para crear el Funko.</param>
    /// <returns>Petición válida o error de dominio.</returns>
    Result<CreateFunkoDto, DomainError> ValidateCreate(CreateFunkoDto request);

    /// <summary>Comprueba que los datos de actualización son correctos.</summary>
    /// <param name="request">Datos para actualizar el Funko.</param>
    /// <returns>Petición válida o error de dominio.</returns>
    Result<UpdateFunkoDto, DomainError> ValidateUpdate(UpdateFunkoDto request);
}
