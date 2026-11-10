using FunkoAppi01.Models;

namespace FunkoAppi01.Repository;

public interface IRepositoryCategoria
{
    Task<IReadOnlyList<Categoria>> GetAllAsync();
    Task<Categoria?> GetByIdAsync(long id);
    Task<Categoria> AddAsync(Categoria categoria);
    Task<Categoria?> UpdateAsync(long id, Categoria categoria);
    Task<bool> SoftDeleteAsync(long id);
    Task<bool> ExistsByNameAsync(string nombre, long? excluideId = null);
    Task<bool> HasFunkoAsync(long id);
}