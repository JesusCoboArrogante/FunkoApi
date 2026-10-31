using FluentAssertions;
using FunkoAppi01.Models;
using FunkoAppi01.Repository;
using NUnit.Framework;

namespace FunkoApp.Test.Repository;

/// <summary>
/// Tests de RepositoryFunko. En memoria: no hace falta TestContainers hasta que la
/// persistencia pase a EF Core (fase 9 de la práctica).
/// </summary>
[TestFixture]
public class RepositoryFunkoTests
{
    private RepositoryFunko _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new RepositoryFunko();
    }

    private static Funko CrearFunko(string nombre = "Pikachu") => new()
    {
        Nombre = nombre,
        Precio = 15.5m,
        Stock = 10,
        Categoria = "Pokemon"
    };

    [TestFixture]
    public class CasosPositivos : RepositoryFunkoTests
    {
        [Test]
        public void Add_DeberiaAsignarIdYFechaDeCreacion()
        {
            // Act
            var creado = _repository.Add(CrearFunko());

            // Assert
            creado.Id.Should().BeGreaterThan(0);
            creado.CreatedAt.Should().NotBe(default);
        }

        [Test]
        public void GetAll_SoloDeberiaDevolverLosNoBorrados()
        {
            // Arrange
            var activo = _repository.Add(CrearFunko("Pikachu"));
            var borrado = _repository.Add(CrearFunko("Charmander"));
            _repository.SoftDelete(borrado.Id);

            // Act
            var resultado = _repository.GetAll();

            // Assert
            resultado.Should().ContainSingle(f => f.Id == activo.Id);
        }

        [Test]
        public void ExistsByName_ExcluyendoSuPropioId_DeberiaIgnorarseAsiMismo()
        {
            // Arrange
            var funko = _repository.Add(CrearFunko("Pikachu"));

            // Act
            var existe = _repository.ExistsByName("Pikachu", excludeId: funko.Id);

            // Assert
            existe.Should().BeFalse();
        }
    }

    [TestFixture]
    public class CasosNegativos : RepositoryFunkoTests
    {
        [Test]
        public void GetById_FunkoBorrado_DeberiaDevolverNull()
        {
            // Arrange
            var creado = _repository.Add(CrearFunko());
            _repository.SoftDelete(creado.Id);

            // Act
            var resultado = _repository.GetById(creado.Id);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void SoftDelete_IdInexistente_DeberiaDevolverFalse()
        {
            // Act
            var resultado = _repository.SoftDelete(999);

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public void Update_IdInexistente_DeberiaDevolverNull()
        {
            // Act
            var resultado = _repository.Update(999, CrearFunko());

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void ExistsByName_OtroFunkoActivoConEseNombre_DeberiaDevolverTrue()
        {
            // Arrange
            _repository.Add(CrearFunko("Pikachu"));

            // Act
            var existe = _repository.ExistsByName("Pikachu");

            // Assert
            existe.Should().BeTrue();
        }
    }
}
