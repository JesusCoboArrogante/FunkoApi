using FluentAssertions;
using FunkoAppi01.Entity;
using FunkoAppi01.Models;
using FunkoAppi01.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FunkoApp.Test.Repository;

/// <summary>
/// Tests de RepositoryFunko contra una base SQLite real en memoria (no mockeada).
/// TestContainers queda para cuando se añada PostgreSQL; SQLite no necesita Docker.
/// </summary>
[TestFixture]
public class RepositoryFunkoTests
{
    private SqliteConnection _connection = null!;
    private AppDbContext _context = null!;
    private RepositoryFunko _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new RepositoryFunko(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _connection.Dispose();
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
        public async Task AddAsync_DeberiaAsignarIdYFechaDeCreacion()
        {
            // Act
            var creado = await _repository.AddAsync(CrearFunko());

            // Assert
            creado.Id.Should().BeGreaterThan(0);
            creado.CreatedAt.Should().NotBe(default);
        }

        [Test]
        public async Task AddAsync_DosFunkosConLaMismaCategoria_DeberianCompartirLaFilaDeCategoria()
        {
            // Act
            await _repository.AddAsync(CrearFunko("Pikachu"));
            await _repository.AddAsync(CrearFunko("Charmander"));

            // Assert
            (await _context.Categorias.CountAsync()).Should().Be(1);
        }

        [Test]
        public async Task GetAllAsync_SoloDeberiaDevolverLosNoBorrados()
        {
            // Arrange
            var activo = await _repository.AddAsync(CrearFunko("Pikachu"));
            var borrado = await _repository.AddAsync(CrearFunko("Charmander"));
            await _repository.SoftDeleteAsync(borrado.Id);

            // Act
            var resultado = await _repository.GetAllAsync();

            // Assert
            resultado.Should().ContainSingle(f => f.Id == activo.Id);
        }

        [Test]
        public async Task ExistsByNameAsync_ExcluyendoSuPropioId_DeberiaIgnorarseAsiMismo()
        {
            // Arrange
            var funko = await _repository.AddAsync(CrearFunko("Pikachu"));

            // Act
            var existe = await _repository.ExistsByNameAsync("Pikachu", excludeId: funko.Id);

            // Assert
            existe.Should().BeFalse();
        }
    }

    [TestFixture]
    public class CasosNegativos : RepositoryFunkoTests
    {
        [Test]
        public async Task GetByIdAsync_FunkoBorrado_DeberiaDevolverNull()
        {
            // Arrange
            var creado = await _repository.AddAsync(CrearFunko());
            await _repository.SoftDeleteAsync(creado.Id);

            // Act
            var resultado = await _repository.GetByIdAsync(creado.Id);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task SoftDeleteAsync_IdInexistente_DeberiaDevolverFalse()
        {
            // Act
            var resultado = await _repository.SoftDeleteAsync(999);

            // Assert
            resultado.Should().BeFalse();
        }

        [Test]
        public async Task UpdateAsync_IdInexistente_DeberiaDevolverNull()
        {
            // Act
            var resultado = await _repository.UpdateAsync(999, CrearFunko());

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public async Task ExistsByNameAsync_OtroFunkoActivoConEseNombre_DeberiaDevolverTrue()
        {
            // Arrange
            await _repository.AddAsync(CrearFunko("Pikachu"));

            // Act
            var existe = await _repository.ExistsByNameAsync("Pikachu");

            // Assert
            existe.Should().BeTrue();
        }
    }
}
