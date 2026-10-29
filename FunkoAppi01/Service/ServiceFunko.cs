using FunkoAppi01.Models;
using FunkoAppi01.Repository;

namespace FunkoAppi01.Service;

public class ServiceFunko(IRepositoryFunko repository) : IServiceFunko
{
    public IEnumerable<Funko> GetAll() => repository.GetAll();

    public Funko? GetById(long id) => repository.GetById(id);

    public Funko Add(Funko funko)
    {
        if (string.IsNullOrWhiteSpace(funko.Nombre))
            throw new ArgumentException("El nombre es obligatorio");

        if (funko.Precio < 0)
            throw new ArgumentException("El precio no puede ser negativo");
        
        return repository.Add(funko);
    }

    public Funko? Update(long id, Funko funko)
    {
        if (string.IsNullOrWhiteSpace(funko.Nombre))
            throw new ArgumentException("El nombre es obligatorio");

        if (funko.Precio < 0)
            throw new ArgumentException("El precio no puede ser negativo");
        
        return repository.Update(id, funko);
    }

    public Funko? UpdatePrice(long id, decimal precio)
    {
        if (precio < 0)
            throw new ArgumentException("El precio no puede ser negativo");
        
        return repository.UpdatePrecio(id, precio);
    }

    public bool Delete(long id) => repository.SoftDelete(id);

    public IEnumerable<Funko> GetByName(string termino) => repository.GetByName(termino);
}