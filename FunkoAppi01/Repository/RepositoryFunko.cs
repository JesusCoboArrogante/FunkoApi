using FunkoAppi01.Models;

namespace FunkoAppi01.Repository;

public class RepositoryFunko: IRepositoryFunko
{
    private readonly Dictionary<long, Funko> _funkos = [];
    private long _nextId = 1; 
    public IReadOnlyList<Funko> GetAll() => Activos();

    public Funko? GetById(long id) => TryGet(id);

    public Funko Add(Funko funko)
    {
        var nuevo = funko with
        {
            Id = _nextId++,
            CreateAt = DateTime.Now
        };
        
        _funkos[nuevo.Id] = nuevo;
        return nuevo;
    }

    public Funko? Update(long id, Funko funko)
    {
        if (TryGet(id) is not { } existente)
        {
            return null;
        }

        var actualizar = existente with
        {
            Nombre = funko.Nombre,
            Precio = funko.Precio,
            Categoria = funko.Categoria,
            Imagen = funko.Imagen,
            UpdateAt = DateTime.Now
        };
        _funkos[actualizar.Id] = actualizar;
        return actualizar;
    }

    public bool SoftDelete(long id)
    {
        if (TryGet(id) is not { } existente)
        {
            return false;
        }

        _funkos[id] = existente with { DeleteAt = DateTime.Now};
        
        return true;
    }

    public Funko? UpdatePrecio(long id, decimal precio)
    {
        if (TryGet(id) is not { } existente)
        {
            return null;
        }

        var actualizar = existente with
        {
            Precio = precio,
            UpdateAt = DateTime.Now
        };
        
        _funkos[actualizar.Id] = actualizar;
        return actualizar;
    }

    public IReadOnlyList<Funko> GetByName(string? nombre)
    {
        var texto = nombre ?? string.Empty;
        return Activos()
            .Where(p => p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Nombre)
            .ToList();
    }

    private List<Funko> Activos() => [.. _funkos.Values.Where(p => p.IsActivo)];
    private  Funko? TryGet(long id) => _funkos.TryGetValue(id, out Funko? funko) && funko.IsActivo ? funko : null;
}