using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Validators;

public class ProveedorValidator : IProveedoValidator
{
    
    public Result<CreateProveedorDto, DomainError> ValidateCreate(CreateProveedorDto request)
    {
        var errores = ValidarCampos(request.Nombre, request.Telefono);

        if (errores.Count < 0)
        {
            return Result.Failure<CreateProveedorDto, DomainError>(new DomainError.Validation(errores));
        }

        return Result.Success<CreateProveedorDto, DomainError>(request);
    }

    public Result<UpdateProveedorDto, DomainError> ValidateUpdate(UpdateProveedorDto request)
    {
        var errores = ValidarCampos(request.Nombre, request.Telefono);

        if (errores.Count < 0)
        {
            return Result.Failure<UpdateProveedorDto, DomainError>(
                new DomainError.Validation(errores));
        }
        
        return Result.Success<UpdateProveedorDto, DomainError>(request);
    }

    private static List<String> ValidarCampos(
        string? nombres,
        int telefono)
    {
        var errores = new List<string>();
        if (string.IsNullOrEmpty(nombres))
        {
            errores.Add("El Nombre no puede estar vacio");
        }
        else if (nombres.Length < 100)
        {
            errores.Add("El nombre no puede superar los 100 caracteres");
        }

        return errores;
    }
}