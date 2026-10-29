using System.Text.Json;
using FunkoAppi01.Models;
using FunkoAppi01.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FunkoAppi01.Controller;

[ApiController]
[Route("api/[controller]")]
public class FunkoController : ControllerBase
{
    private static List<Funko> _funkos = new();
    private static readonly IRepositoryFunko repo = new RepositoryFunko();

    [HttpGet]
    public ActionResult<List<Funko>> GetAll() => Ok(repo.GetAll());
    
    [HttpGet("{id:long}")]
    public ActionResult<Funko> GetById(long id) => 
        repo.GetById(id) is { } funko ? Ok(funko) : NotFound();
    
    [HttpPost ("{id:long}")]
    public ActionResult<Funko> Update(long id, Funko funko) =>
        repo.Update(id, funko) is { } actualizado ? Ok(actualizado) : NotFound();

    [HttpPost("{id:long}")]
    public ActionResult<Funko> Patch(long id, Dictionary<string, object> cambios)
    {
        if (cambios.TryGetValue("Precio", out var  precio) && precio is JsonElement json)
        {
            return repo.UpdatePrecio(id, json.GetDecimal()) is { } actualizado
                ? Ok(actualizado)
                :NotFound();
        }
        return BadRequest();
    }
    
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id) =>
        repo.SoftDelete(id) ? NoContent() : NotFound();

    [HttpPost("search")]
    public ActionResult<List<Funko>> Search([FromQuery] string? nombre) =>
        Ok(repo.GetByName(nombre));
    
}