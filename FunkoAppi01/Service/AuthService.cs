using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Entity;
using FunkoAppi01.Errors;
using FunkoAppi01.Repository;
using Microsoft.Extensions.Logging;

namespace FunkoAppi01.Service;

/// <summary>
/// Orquesta el registro y el login: hashea/verifica con BCrypt y emite el JWT.
/// </summary>
public class AuthService(
    IRepositoryUser repository,
    JwtService jwtService,
    ILogger<AuthService> logger) : IAuthService
{
    private const int BCryptWorkFactor = 11;

    public async Task<Result<AuthResponseDto, DomainError>> SignupAsync(SignupDto request)
    {
        if (await repository.FindByEmailAsync(request.Email) is not null)
            return Result.Failure<AuthResponseDto, DomainError>(
                AuthErrors.Conflict("El email ya está registrado"));

        var user = new UserEntity
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, BCryptWorkFactor),
            Role = UserRoles.User,
            CreatedAt = DateTime.UtcNow
        };

        user = await repository.CreateAsync(user);
        logger.LogInformation("Usuario registrado: {Id} ({Username})", user.Id, user.Username);

        return Result.Success<AuthResponseDto, DomainError>(ToAuthResponse(user));
    }

    public async Task<Result<AuthResponseDto, DomainError>> LoginAsync(LoginDto request)
    {
        var user = await repository.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Intento de login fallido para: {Email}", request.Email);
            return Result.Failure<AuthResponseDto, DomainError>(
                AuthErrors.Unauthorized("Credenciales inválidas"));
        }

        user.LastLoginAt = DateTime.UtcNow;
        await repository.UpdateAsync(user);

        logger.LogInformation("Login correcto: {Id} ({Username})", user.Id, user.Username);
        return Result.Success<AuthResponseDto, DomainError>(ToAuthResponse(user));
    }

    private AuthResponseDto ToAuthResponse(UserEntity user) => new(
        jwtService.GenerateToken(user),
        "Bearer",
        jwtService.AccessTokenExpirySeconds,
        user.Id,
        user.Username,
        user.Role);
}
