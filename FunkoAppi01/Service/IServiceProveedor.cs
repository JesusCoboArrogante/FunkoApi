using CSharpFunctionalExtensions;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Service;
using FunkoAppi01.Dto;
public interface IServiceProveedor
{
    Task <IEnumerable<ProveedorDto>> GetAllAsync();
    Task<Result<ProveedorDto, DomainError>> GetByIdAsync(string id);
    Task<Result<ProveedorDto,DomainError>> AddAsync(CreateProveedorDto request);
    Task<Result<ProveedorDto, DomainError>> UpdateAsync(string id, UpdateProveedorDto request);
    Task<Result<bool, DomainError>> DeleteAsync(string id);
}