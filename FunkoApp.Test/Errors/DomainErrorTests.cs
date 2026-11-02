using FluentAssertions;
using FunkoAppi01.Errors;
using NUnit.Framework;

namespace FunkoApp.Test.Errors;

/// <summary>
/// Tests de DomainError.
/// </summary>
[TestFixture]
public class DomainErrorTests
{
    [Test]
    public void NotFound_DeberiaIncluirElIdEnElMensaje()
    {
        // Act
        var error = FunkoErrors.NotFound(5);

        // Assert
        error.Should().BeOfType<DomainError.NotFound>();
        error.Message.Should().Contain("5");
    }

    [Test]
    public void Validation_DeberiaUnirTodosLosErrores()
    {
        // Act
        var error = FunkoErrors.Validation(["El nombre es obligatorio", "El precio no puede ser negativo"]);

        // Assert
        error.Should().BeOfType<DomainError.Validation>();
        error.Message.Should().Be("El nombre es obligatorio, El precio no puede ser negativo");
    }

    [Test]
    public void AlreadyExists_DeberiaIncluirElNombreEnElMensaje()
    {
        // Act
        var error = FunkoErrors.AlreadyExists("Pikachu");

        // Assert
        error.Should().BeOfType<DomainError.AlreadyExists>();
        error.Message.Should().Contain("Pikachu");
    }
}
