using FunkoAppi01.Models;

namespace FunkoAppi01.Repository;

public interface IRepositoryProveedor
{
    Task<IReadOnlyList<Proveedor>> GetAllAsync();
    Task<Proveedor?> GetByIdAsync(string id);
    Task<Proveedor> AddAsync(Proveedor proveedor);
    Task<Proveedor?> UpdateAsync(string id, Proveedor proveedor);
    Task<bool> SoftDeleteAsync(string id);
    Task<IReadOnlyList<Proveedor>> GetByNameAsync(string? nombre);
    Task<bool> ExistsByNameAsync(string nombre, string? excludeId = null);
    Task<bool> HasFunkoAsync(string id);
}