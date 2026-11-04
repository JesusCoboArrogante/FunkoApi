using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Validators;

public interface ICategoriaValidator
{
    Result<CreateCategoriaDto, DomainError> ValidateCreate(
        CreateCategoriaDto request);

    Result<UpdateCategoriaDto, DomainError> ValidateUpdate(
        UpdateCategoriaDto request);
}