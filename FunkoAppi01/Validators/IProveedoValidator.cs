using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Validators;

public interface IProveedoValidator
{
    Result <CreateProveedorDto, DomainError> ValidateCreate(CreateProveedorDto request);
    Result<UpdateProveedorDto, DomainError> ValidateUpdate(UpdateProveedorDto request);
}