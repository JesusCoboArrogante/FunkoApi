using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;
using FunkoAppi01.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FunkoAppi01.Controller;

/// <summary>
/// Autenticación: registro, login y perfil del usuario autenticado.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponseDto>> Signup(SignupDto request)
    {
        var resultado = await authService.SignupAsync(request);
        return resultado.IsSuccess
            ? CreatedAtAction(nameof(GetCurrentUser), resultado.Value)
            : ToActionResult(resultado);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto request) =>
        ToActionResult(await authService.LoginAsync(request));

    [Authorize]
    [HttpGet("me")]
    public ActionResult GetCurrentUser() => Ok(new
    {
        Id = User.FindFirst("sub")?.Value,
        Username = User.FindFirst("username")?.Value,
        Email = User.FindFirst("email")?.Value,
        Role = User.FindFirst("role")?.Value
    });

    private ActionResult<T> ToActionResult<T>(Result<T, DomainError> resultado)
    {
        if (resultado.IsSuccess)
            return Ok(resultado.Value);

        return resultado.Error switch
        {
            DomainError.Unauthorized unauthorized => Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado",
                Detail = unauthorized.Message
            }),
            DomainError.Conflict conflicto => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflicto",
                Detail = conflicto.Message
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
