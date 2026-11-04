using FunkoAppi01.Entity;
using FunkoAppi01.Models;
using Microsoft.EntityFrameworkCore;

namespace FunkoAppi01.Repository;

public class RepositoryCategoria (AppDbContext context) : IRepositoryCategoria
{
    public async Task<IReadOnlyList<Categoria>> GetAllAsync()
    {
        return await context.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new Categoria
            {
                Id = c.Id,
                Nombre = c.Nombre,
            })
            .ToListAsync();
    }

    public async Task<Categoria?> GetByIdAsync(long id)
    {
        return await context.Categorias
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new Categoria
            {
                Id = c.Id,
                Nombre = c.Nombre,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Categoria> AddAsync(Categoria categoria)
    {
        var entidad = new CategoriaEntity
        {
            Nombre = categoria.Nombre,
        };
        
        context.Categorias.Add(entidad);
        await context.SaveChangesAsync();

        return new Categoria
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre,
        };
    }

    public async Task<Categoria?> UpdateAsync(long id, Categoria categoria)
    {
        var entidad = await context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id);
        if (entidad == null)
            return null;
        entidad.Nombre = categoria.Nombre;
        await context.SaveChangesAsync();
        return new Categoria
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre,
        };
    }

    public async Task<bool> SoftDeleteAsync(long id)
    {
        var entidad = await context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id);
        if (entidad is null)
            return false;
        context.Categorias.Remove(entidad);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string nombre, long? excluideId = null)
    {
        return await context.Categorias.AnyAsync(c =>
            (!excluideId.HasValue || c.Id != excluideId.Value) &&
            c.Nombre.ToLower() == nombre.ToLower()
        );
    }

    public async Task<bool> HasFunkoAsync(long id)
    {
        return await context.Funkos
            .IgnoreQueryFilters()
            .AnyAsync(f => f.CategoriaId == id);
    }
}