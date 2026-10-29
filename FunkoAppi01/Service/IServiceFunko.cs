using FunkoAppi01.Models;

namespace FunkoAppi01.Service;

public interface IServiceFunko
{
    IEnumerable<Funko> GetAll();
    Funko? GetById(long id);
    Funko Add(Funko funko);
    Funko? Update(long id, Funko funko);
    Funko? UpdatePrice(long id, decimal precio);
    bool Delete(long id);

    IEnumerable<Funko> GetByName(string termino);
}