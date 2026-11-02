using FluentAssertions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;
using FunkoAppi01.Validators;
using NUnit.Framework;

namespace FunkoApp.Test.Validators;

/// <summary>
/// Tests de FunkoValidator.
/// </summary>
[TestFixture]
public class FunkoValidatorTests
{
    private FunkoValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new FunkoValidator();
    }

    private static CreateFunkoDto PeticionCrear(
        string nombre = "Pikachu",
        decimal precio = 15.5m,
        int stock = 10,
        string categoria = "Pokemon") =>
        new(nombre, precio, stock, categoria);

    private static UpdateFunkoDto PeticionActualizar(
        string nombre = "Pikachu",
        decimal precio = 15.5m,
        int stock = 10,
        string categoria = "Pokemon") =>
        new(nombre, precio, stock, categoria);

    [TestFixture]
    public class CasosPositivos : FunkoValidatorTests
    {
        [Test]
        public void ValidateCreate_DatosValidos_DeberiaRetornarSuccess()
        {
            // Arrange
            var request = PeticionCrear();

            // Act
            var resultado = _validator.ValidateCreate(request);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(request);
        }

        [Test]
        public void ValidateUpdate_DatosValidos_DeberiaRetornarSuccess()
        {
            // Arrange
            var request = PeticionActualizar();

            // Act
            var resultado = _validator.ValidateUpdate(request);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(request);
        }
    }

    [TestFixture]
    public class CasosNegativos : FunkoValidatorTests
    {
        [Test]
        public void ValidateCreate_NombreSoloEspacios_DeberiaRetornarFailure()
        {
            // Arrange
            var request = PeticionCrear(nombre: "   ");

            // Act
            var resultado = _validator.ValidateCreate(request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.Validation>();
            resultado.Error.Message.Should().Contain("nombre no puede estar vacío");
        }

        [Test]
        public void ValidateCreate_CategoriaSoloEspacios_DeberiaRetornarFailure()
        {
            // Arrange
            var request = PeticionCrear(categoria: "   ");

            // Act
            var resultado = _validator.ValidateCreate(request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Message.Should().Contain("categoría no puede estar vacía");
        }

        [Test]
        public void ValidateCreate_NombreYCategoriaInvalidos_DeberiaDevolverLosDosErrores()
        {
            // Arrange
            var request = PeticionCrear(nombre: "", categoria: "");

            // Act
            var resultado = _validator.ValidateCreate(request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Message.Should()
                .Contain("nombre no puede estar vacío").And
                .Contain("categoría no puede estar vacía");
        }

        [Test]
        public void ValidateUpdate_NombreSoloEspacios_DeberiaRetornarFailure()
        {
            // Arrange
            var request = PeticionActualizar(nombre: "   ");

            // Act
            var resultado = _validator.ValidateUpdate(request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Message.Should().Contain("nombre no puede estar vacío");
        }
    }
}
