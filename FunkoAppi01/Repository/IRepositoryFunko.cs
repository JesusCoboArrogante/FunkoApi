using FunkoAppi01.Models;

namespace FunkoAppi01.Repository;

public interface IRepositoryFunko
{
    IReadOnlyList<Funko> GetAll();
    Funko? GetById(long id);
    Funko Add(Funko funko);
    Funko? Update(long id, Funko funko);
    bool SoftDelete(long id);

    Funko? UpdatePrecio(long id, decimal precio);
    IReadOnlyList<Funko> GetByName(string? nombre);


}