using CSharpFunctionalExtensions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;
using FunkoAppi01.Service;
using Microsoft.AspNetCore.Mvc;

namespace FunkoAppi01.Controller;

[ApiController]
[Route("api/[controller]")]
public class FunkoController(IServiceFunko service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FunkoDto>>> GetAll() => Ok(await service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<FunkoDto>> GetById(long id) => ToActionResult(await service.GetByIdAsync(id));

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<FunkoDto>>> Search([FromQuery] string? nombre) =>
        Ok(await service.GetByNameAsync(nombre ?? string.Empty));

    [HttpPost]
    public async Task<ActionResult<FunkoDto>> Create(CreateFunkoDto request)
    {
        var resultado = await service.AddAsync(request);
        return resultado.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = resultado.Value.Id }, resultado.Value)
            : ToActionResult(resultado);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<FunkoDto>> Update(long id, UpdateFunkoDto request) =>
        ToActionResult(await service.UpdateAsync(id, request));

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<FunkoDto>> UpdatePrice(long id, [FromBody] decimal precio) =>
        ToActionResult(await service.UpdatePriceAsync(id, precio));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        var resultado = await service.DeleteAsync(id);
        return resultado.IsSuccess ? NoContent() : ToActionResult(resultado).Result!;
    }

    private ActionResult<T> ToActionResult<T>(Result<T, DomainError> resultado)
    {
        if (resultado.IsSuccess)
            return Ok(resultado.Value);

        return resultado.Error switch
        {
            DomainError.NotFound notFound => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "No encontrado",
                Detail = notFound.Message
            }),
            DomainError.AlreadyExists duplicado => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflicto",
                Detail = duplicado.Message
            }),
            DomainError.Validation validacion => BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Solicitud inválida",
                Detail = validacion.Message
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
