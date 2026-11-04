using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Validators;

public class CategoriaValidator : ICategoriaValidator
{
    public Result<CreateCategoriaDto, DomainError> ValidateCreate(CreateCategoriaDto request)
    {
        var errores = ValidarNombre(request.Nombre);

        if (errores.Count > 0)
        {
            return Result.Failure<CreateCategoriaDto, DomainError>(
                new DomainError.Validation(errores));
        }

        return Result.Success<CreateCategoriaDto, DomainError>(request);
    }

    public Result<UpdateCategoriaDto, DomainError> ValidateUpdate(UpdateCategoriaDto request)
    {
        var errores = ValidarNombre(request.Nombre);

        if (errores.Count > 0)
        {
            return Result.Failure<UpdateCategoriaDto, DomainError>(
                new DomainError.Validation(errores));
        }

        return Result.Success<UpdateCategoriaDto, DomainError>(request);
    }
    
    private static List<string> ValidarNombre(string? nombre)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errores.Add(
                "El nombre no puede estar vacío ni ser solo espacios");
        }
        else if (nombre.Length > 100)
        {
            errores.Add(
                "El nombre no puede superar los 100 caracteres");
        }

        return errores;
    }
}