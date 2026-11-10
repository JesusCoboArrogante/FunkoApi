using FunkoAppi01.Entity;
using FunkoAppi01.Models;
using  Microsoft.EntityFrameworkCore;

namespace FunkoAppi01.Repository;


public class RepositoryProveedor (ProveedorMongoDbContext context, AppDbContext sqlContext) : IRepositoryProveedor
{
    public async Task<IReadOnlyList<Proveedor>> GetAllAsync()
    {
        return await context.Proveedores
        .AsNoTracking()
        .OrderBy(p=> p.Id)
        .Select(p => new Proveedor
            {
                Id = p.Id,
                Nombre = p.Nombre
            })
        .ToListAsync();
    }

    public async Task<Proveedor?> GetByIdAsync(long id)
    {
        return await context.Proveedores
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new Proveedor()
            {
                Id = p.Id,
                Nombre = p.Nombre
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Proveedor> AddAsync(Proveedor proveedor)
    {
        var entidad = new ProveedorEntity
        {
            Nombre = proveedor.Nombre
        };
        
        context.Proveedores.Add(entidad);
        await context.SaveChangesAsync();

        return new Proveedor
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre
        };
    }

    public async Task<Proveedor?> UpdateAsync(long id, Proveedor proveedor)
    {
        var entidad = await context.Proveedores
            .FirstOrDefaultAsync(p => p.Id == id);
        if (entidad == null)
            return null;
        entidad.Nombre = proveedor.Nombre;
        await context.SaveChangesAsync();
        return new Proveedor
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre
        };
    }

    public async Task<bool> SoftDeleteAsync(long id)
    {
        var entidad = await context.Proveedores
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entidad is null)
            return false;
        context.Proveedores.Remove(entidad);
        await context.SaveChangesAsync();
        return true;
    }

    public Task<IReadOnlyList<Proveedor>> GetByNameAsync(string? nombre)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsByNameAsync(string nombre, long? excludeId = null)
    {
        return await context.Proveedores.AnyAsync(p =>
            (!excludeId.HasValue || p.Id != excludeId.Value) &&
            p.Nombre.ToLower() == nombre.ToLower()
        );
    }

    public async Task<bool> HasFunkoAsync(long id)
    {
        return await sqlContext.Funkos
            .IgnoreQueryFilters()
            .AnyAsync(f => f.ProveedorId == id);
    }
}