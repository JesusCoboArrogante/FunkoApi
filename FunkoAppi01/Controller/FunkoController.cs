using FunkoAppi01.Models;
using FunkoAppi01.Service;
using Microsoft.AspNetCore.Mvc;

namespace FunkoAppi01.Controller;

[ApiController]
[Route("api/[controller]")]
public class FunkoController(IServiceFunko service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Funko>> GetAll() => Ok(service.GetAll());

    [HttpGet("{id:long}")]
    public ActionResult<Funko> GetById(long id) =>
        service.GetById(id) is { } funko ? Ok(funko) : NotFound();

    [HttpGet("search")]
    public ActionResult<IEnumerable<Funko>> Search([FromQuery] string? nombre) =>
        Ok(service.GetByName(nombre ?? string.Empty));

    [HttpPost]
    public ActionResult<Funko> Create(Funko funko)
    {
        var creado = service.Add(funko);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:long}")]
    public ActionResult<Funko> Update(long id, Funko funko) =>
        service.Update(id, funko) is { } actualizado ? Ok(actualizado) : NotFound();

    [HttpPatch("{id:long}")]
    public ActionResult<Funko> UpdatePrice(long id, [FromBody] decimal precio) =>
        service.UpdatePrice(id, precio) is { } actualizado ? Ok(actualizado) : NotFound();

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id) =>
        service.Delete(id) ? NoContent() : NotFound();
}
