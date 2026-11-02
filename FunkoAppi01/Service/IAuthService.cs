using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;

namespace FunkoAppi01.Service;

public interface IAuthService
{
    Task<Result<AuthResponseDto, DomainError>> SignupAsync(SignupDto request);
    Task<Result<AuthResponseDto, DomainError>> LoginAsync(LoginDto request);
}
