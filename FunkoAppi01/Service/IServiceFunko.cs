using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Service;

public interface IServiceFunko
{
    IEnumerable<FunkoDto> GetAll();
    Result<FunkoDto, DomainError> GetById(long id);
    Result<FunkoDto, DomainError> Add(CreateFunkoDto request);
    Result<FunkoDto, DomainError> Update(long id, UpdateFunkoDto request);
    Result<FunkoDto, DomainError> UpdatePrice(long id, decimal precio);
    Result<bool, DomainError> Delete(long id);
    IEnumerable<FunkoDto> GetByName(string termino);
}
