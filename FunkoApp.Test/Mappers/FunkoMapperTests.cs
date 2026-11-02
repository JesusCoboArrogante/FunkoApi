using FluentAssertions;
using FunkoAppi01.Dto;
using FunkoAppi01.Mappers;
using FunkoAppi01.Models;
using NUnit.Framework;

namespace FunkoApp.Test.Mappers;

/// <summary>
/// Tests de FunkoMapper.
/// </summary>
[TestFixture]
public class FunkoMapperTests
{
    [TestFixture]
    public class CasosPositivos : FunkoMapperTests
    {
        [Test]
        public void ToFunko_DesdeCreateFunkoDto_DeberiaCopiarTodosLosCampos()
        {
            // Arrange
            var dto = new CreateFunkoDto("Pikachu", 15.5m, 10, "Pokemon");

            // Act
            var funko = dto.ToFunko();

            // Assert
            funko.Nombre.Should().Be("Pikachu");
            funko.Precio.Should().Be(15.5m);
            funko.Stock.Should().Be(10);
            funko.Categoria.Should().Be("Pokemon");
        }

        [Test]
        public void ToFunko_DesdeUpdateFunkoDto_DeberiaCopiarTodosLosCampos()
        {
            // Arrange
            var dto = new UpdateFunkoDto("Charmander", 20m, 5, "Pokemon");

            // Act
            var funko = dto.ToFunko();

            // Assert
            funko.Nombre.Should().Be("Charmander");
            funko.Precio.Should().Be(20m);
            funko.Stock.Should().Be(5);
            funko.Categoria.Should().Be("Pokemon");
        }

        [Test]
        public void ToDto_DesdeFunko_DeberiaCopiarTodosLosCampos()
        {
            // Arrange
            var creado = new DateTime(2026, 1, 1);
            var funko = new Funko
            {
                Id = 1,
                Nombre = "Bulbasaur",
                Precio = 12m,
                Stock = 3,
                Categoria = "Pokemon",
                CreatedAt = creado,
                UpdatedAt = null
            };

            // Act
            var dto = funko.ToDto();

            // Assert
            dto.Id.Should().Be(1);
            dto.Nombre.Should().Be("Bulbasaur");
            dto.Precio.Should().Be(12m);
            dto.Stock.Should().Be(3);
            dto.Categoria.Should().Be("Pokemon");
            dto.CreatedAt.Should().Be(creado);
            dto.UpdatedAt.Should().BeNull();
        }
    }
}
