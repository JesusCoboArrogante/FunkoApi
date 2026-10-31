using CSharpFunctionalExtensions;
using FluentAssertions;
using FunkoAppi01.Dto;
using FunkoAppi01.Errors;
using FunkoAppi01.Models;
using FunkoAppi01.Repository;
using FunkoAppi01.Service;
using FunkoAppi01.Validators;
using Moq;
using NUnit.Framework;

namespace FunkoApp.Test.Services;

/// <summary>
/// Tests unitarios de ServiceFunko. El repositorio y el validador van simulados con Moq.
/// </summary>
[TestFixture]
public class ServiceFunkoTests
{
    private Mock<IRepositoryFunko> _mockRepository = null!;
    private Mock<IFunkoValidator> _mockValidator = null!;
    private ServiceFunko _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IRepositoryFunko>();
        _mockValidator = new Mock<IFunkoValidator>();
        _service = new ServiceFunko(_mockRepository.Object, _mockValidator.Object);
    }

    private static Funko CrearFunko(long id = 1, string nombre = "Pikachu") => new()
    {
        Id = id,
        Nombre = nombre,
        Precio = 15.5m,
        Stock = 10,
        Categoria = "Pokemon",
        CreatedAt = new DateTime(2026, 1, 1)
    };

    private static CreateFunkoDto CrearCreateDto(string nombre = "Pikachu") =>
        new(nombre, 15.5m, 10, "Pokemon");

    private static UpdateFunkoDto CrearUpdateDto(string nombre = "Pikachu") =>
        new(nombre, 20m, 5, "Pokemon");

    [TestFixture]
    public class CasosPositivos : ServiceFunkoTests
    {
        [Test]
        public void GetAll_DeberiaMapearTodosLosFunkos()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAll())
                .Returns([CrearFunko(1, "Pikachu"), CrearFunko(2, "Charmander")]);

            // Act
            var resultado = _service.GetAll();

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Test]
        public void GetById_ExisteElFunko_DeberiaRetornarSuccess()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(1)).Returns(CrearFunko());

            // Act
            var resultado = _service.GetById(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Pikachu");
        }

        [Test]
        public void Add_DatosValidos_DeberiaLlamarAlRepositorioYRetornarSuccess()
        {
            // Arrange
            var request = CrearCreateDto();
            _mockValidator.Setup(v => v.ValidateCreate(request))
                .Returns(Result.Success<CreateFunkoDto, DomainError>(request));
            _mockRepository.Setup(r => r.ExistsByName(request.Nombre, null)).Returns(false);
            _mockRepository.Setup(r => r.Add(It.IsAny<Funko>())).Returns(CrearFunko());

            // Act
            var resultado = _service.Add(request);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _mockRepository.Verify(r => r.Add(It.IsAny<Funko>()), Times.Once);
        }

        [Test]
        public void Update_ExisteElFunko_DeberiaRetornarSuccess()
        {
            // Arrange
            var request = CrearUpdateDto();
            _mockValidator.Setup(v => v.ValidateUpdate(request))
                .Returns(Result.Success<UpdateFunkoDto, DomainError>(request));
            _mockRepository.Setup(r => r.ExistsByName(request.Nombre, 1)).Returns(false);
            _mockRepository.Setup(r => r.Update(1, It.IsAny<Funko>())).Returns(CrearFunko());

            // Act
            var resultado = _service.Update(1, request);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void UpdatePrice_ExisteElFunko_DeberiaRetornarSuccess()
        {
            // Arrange
            _mockRepository.Setup(r => r.UpdatePrecio(1, 25m)).Returns(CrearFunko());

            // Act
            var resultado = _service.UpdatePrice(1, 25m);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Delete_ExisteElFunko_DeberiaRetornarSuccess()
        {
            // Arrange
            _mockRepository.Setup(r => r.SoftDelete(1)).Returns(true);

            // Act
            var resultado = _service.Delete(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosNegativos : ServiceFunkoTests
    {
        [Test]
        public void GetById_NoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(99)).Returns((Funko?)null);

            // Act
            var resultado = _service.GetById(99);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.NotFound>();
        }

        [Test]
        public void Add_ValidacionFalla_NoDeberiaLlamarAlRepositorioAdd()
        {
            // Arrange
            var request = CrearCreateDto(nombre: "");
            _mockValidator.Setup(v => v.ValidateCreate(request))
                .Returns(Result.Failure<CreateFunkoDto, DomainError>(
                    FunkoErrors.Validation(["El nombre no puede estar vacío"])));

            // Act
            var resultado = _service.Add(request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.Validation>();
            _mockRepository.Verify(r => r.Add(It.IsAny<Funko>()), Times.Never);
        }

        [Test]
        public void Add_NombreDuplicado_NoDeberiaLlamarAlRepositorioAdd()
        {
            // Arrange
            var request = CrearCreateDto();
            _mockValidator.Setup(v => v.ValidateCreate(request))
                .Returns(Result.Success<CreateFunkoDto, DomainError>(request));
            _mockRepository.Setup(r => r.ExistsByName(request.Nombre, null)).Returns(true);

            // Act
            var resultado = _service.Add(request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.AlreadyExists>();
            _mockRepository.Verify(r => r.Add(It.IsAny<Funko>()), Times.Never);
        }

        [Test]
        public void Update_NoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            var request = CrearUpdateDto();
            _mockValidator.Setup(v => v.ValidateUpdate(request))
                .Returns(Result.Success<UpdateFunkoDto, DomainError>(request));
            _mockRepository.Setup(r => r.ExistsByName(request.Nombre, 99)).Returns(false);
            _mockRepository.Setup(r => r.Update(99, It.IsAny<Funko>())).Returns((Funko?)null);

            // Act
            var resultado = _service.Update(99, request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.NotFound>();
        }

        [Test]
        public void UpdatePrice_PrecioNegativo_NoDeberiaLlamarAlRepositorio()
        {
            // Act
            var resultado = _service.UpdatePrice(1, -5m);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.Validation>();
            _mockRepository.Verify(r => r.UpdatePrecio(It.IsAny<long>(), It.IsAny<decimal>()), Times.Never);
        }

        [Test]
        public void Delete_NoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.SoftDelete(99)).Returns(false);

            // Act
            var resultado = _service.Delete(99);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.NotFound>();
        }
    }
}
