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
    public ActionResult<IEnumerable<FunkoDto>> GetAll() => Ok(service.GetAll());

    [HttpGet("{id:long}")]
    public ActionResult<FunkoDto> GetById(long id) => ToActionResult(service.GetById(id));

    [HttpGet("search")]
    public ActionResult<IEnumerable<FunkoDto>> Search([FromQuery] string? nombre) =>
        Ok(service.GetByName(nombre ?? string.Empty));

    [HttpPost]
    public ActionResult<FunkoDto> Create(CreateFunkoDto request)
    {
        var resultado = service.Add(request);
        return resultado.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = resultado.Value.Id }, resultado.Value)
            : ToActionResult(resultado);
    }

    [HttpPut("{id:long}")]
    public ActionResult<FunkoDto> Update(long id, UpdateFunkoDto request) =>
        ToActionResult(service.Update(id, request));

    [HttpPatch("{id:long}")]
    public ActionResult<FunkoDto> UpdatePrice(long id, [FromBody] decimal precio) =>
        ToActionResult(service.UpdatePrice(id, precio));

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var resultado = service.Delete(id);
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
