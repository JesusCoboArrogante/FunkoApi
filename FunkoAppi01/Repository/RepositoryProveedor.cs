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
                Nombre = p.Nombre,
                Telefono = p.Telefono
            })
        .ToListAsync();
    }

    

    public async Task<Proveedor?> GetByIdAsync(string id)
    {
        return await context.Proveedores
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new Proveedor()
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Telefono = p.Telefono
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Proveedor> AddAsync(Proveedor proveedor)
    {
        var entidad = new ProveedorEntity
        {
            Nombre = proveedor.Nombre,
            Telefono = proveedor.Telefono
        };
        
        context.Proveedores.Add(entidad);
        await context.SaveChangesAsync();

        return new Proveedor
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre,
            Telefono = entidad.Telefono
        };
    }

    public async Task<Proveedor?> UpdateAsync(string id, Proveedor proveedor)
    {
        var entidad = await context.Proveedores
            .FirstOrDefaultAsync(p => p.Id == id);
        if (entidad == null)
            return null;
        entidad.Nombre = proveedor.Nombre;
        entidad.Telefono = proveedor.Telefono;
        await context.SaveChangesAsync();
        return new Proveedor
        {
            Id = entidad.Id,
            Nombre = entidad.Nombre,
            Telefono = entidad.Telefono
        };
    }

    public async Task<bool> SoftDeleteAsync(string id)
    {
        var entidad = await context.Proveedores
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entidad is null)
            return false;
        context.Proveedores.Remove(entidad);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<Proveedor>> GetByNameAsync(string? nombre)
    {
        var texto = nombre ?? string.Empty;

        var proveedor = await GetAllAsync();

        return proveedor
            .Where(p => p.Nombre.Contains(
                texto,
                StringComparison.OrdinalIgnoreCase
            )).ToList();
    }

    

    public async Task<bool> ExistsByNameAsync(string nombre, string? excludeId = null)
    {
        return await context.Proveedores.AnyAsync(p =>
            (excludeId == null || p.Id != excludeId) &&
            p.Nombre.ToLower() == nombre.ToLower()
        );
    }

    public async Task<bool> HasFunkoAsync(string id)
    {
        return await sqlContext.Funkos
            .IgnoreQueryFilters()
            .AnyAsync(f => f.ProveedorId == id);
    }
}