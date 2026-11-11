using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Entity;
using FunkoAppi01.Errors;
using FunkoAppi01.Service;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FunkoAppi01.Controller;

[ApiController]
[Route("api/funko/proveedor")]
[Microsoft.AspNetCore.Authorization.Authorize(Roles = UserRoles.Admin)]
public class ProveedorController(IServiceProveedor service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetAll()
    {
        return Ok(await service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProveedorDto>> GetById(string id)
    {
        return ToActionResult(await service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<ProveedorDto>> Create(CreateProveedorDto request)
    {
        var resultado = await service.AddAsync(request);

        return resultado.IsSuccess
            ? CreatedAtRoute(
                nameof(GetById),
                new { id = resultado.Value.Id },
                resultado.Value)
            : ToActionResult(resultado);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProveedorDto>> Update(string id, UpdateProveedorDto request)
    {
        
        return ToActionResult(await service.UpdateAsync(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var resultado = await service.DeleteAsync(id);
        return resultado.IsSuccess
            ? NoContent()
            : ToActionResult(resultado).Result!;
    }
    


    private ActionResult<T> ToActionResult<T>(Result<T, DomainError> resultado)
    {
        if (resultado.IsSuccess)
            return Ok(resultado.Value);
        
        return resultado.Error switch
        {
            DomainError.NotFound noEncontrado => NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "No encontrado",
                    Detail = noEncontrado.Message 
                }),
            DomainError.AlreadyExists duplicado => Conflict(
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflicto",
                    Detail = duplicado.Message
                }),

            DomainError.Conflict conflicto => Conflict(
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflicto",
                    Detail = conflicto.Message
                }),

            DomainError.Validation validacion => BadRequest(
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Solicitud inválida",
                    Detail = validacion.Message
                }),

            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
        }
    }

