using FluentAssertions;
using FunkoAppi01.Config;
using FunkoAppi01.Dto;
using FunkoAppi01.Entity;
using FunkoAppi01.Errors;
using FunkoAppi01.Repository;
using FunkoAppi01.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace FunkoApp.Test.Services;

/// <summary>
/// Tests unitarios de AuthService. El repositorio va simulado con Moq.
/// </summary>
[TestFixture]
public class AuthServiceTests
{
    private Mock<IRepositoryUser> _mockRepository = null!;
    private AuthService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IRepositoryUser>();

        var jwtConfig = Options.Create(new JwtConfig
        {
            Secret = "esta-es-una-clave-de-prueba-de-al-menos-32-bytes"
        });

        _service = new AuthService(
            _mockRepository.Object,
            new JwtService(jwtConfig),
            new Mock<ILogger<AuthService>>().Object);
    }

    private static UserEntity CrearUsuario(string email = "admin@funko.com", string password = "admin123") => new()
    {
        Id = 1,
        Username = "admin",
        Email = email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, 11),
        Role = UserRoles.Admin
    };

    [TestFixture]
    public class CasosPositivos : AuthServiceTests
    {
        [Test]
        public async Task SignupAsync_EmailNuevo_DeberiaCrearElUsuarioYDevolverToken()
        {
            // Arrange
            var request = new SignupDto("cristian", "cristian@funko.com", "MiClave123");
            _mockRepository.Setup(r => r.FindByEmailAsync(request.Email)).ReturnsAsync((UserEntity?)null);
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<UserEntity>()))
                .ReturnsAsync((UserEntity u) => u);

            // Act
            var resultado = await _service.SignupAsync(request);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.AccessToken.Should().NotBeNullOrEmpty();
            resultado.Value.Role.Should().Be(UserRoles.User);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<UserEntity>()), Times.Once);
        }

        [Test]
        public async Task LoginAsync_CredencialesCorrectas_DeberiaDevolverToken()
        {
            // Arrange
            var usuario = CrearUsuario();
            _mockRepository.Setup(r => r.FindByEmailAsync(usuario.Email)).ReturnsAsync(usuario);

            // Act
            var resultado = await _service.LoginAsync(new LoginDto(usuario.Email, "admin123"));

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Username.Should().Be("admin");
            resultado.Value.Role.Should().Be(UserRoles.Admin);
            _mockRepository.Verify(r => r.UpdateAsync(usuario), Times.Once);
        }
    }

    [TestFixture]
    public class CasosNegativos : AuthServiceTests
    {
        [Test]
        public async Task SignupAsync_EmailYaRegistrado_NoDeberiaCrearElUsuario()
        {
            // Arrange
            var request = new SignupDto("otro", "admin@funko.com", "MiClave123");
            _mockRepository.Setup(r => r.FindByEmailAsync(request.Email)).ReturnsAsync(CrearUsuario());

            // Act
            var resultado = await _service.SignupAsync(request);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.Conflict>();
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<UserEntity>()), Times.Never);
        }

        [Test]
        public async Task LoginAsync_PasswordIncorrecta_DeberiaRetornarUnauthorized()
        {
            // Arrange
            var usuario = CrearUsuario();
            _mockRepository.Setup(r => r.FindByEmailAsync(usuario.Email)).ReturnsAsync(usuario);

            // Act
            var resultado = await _service.LoginAsync(new LoginDto(usuario.Email, "password-incorrecta"));

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.Unauthorized>();
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>()), Times.Never);
        }

        [Test]
        public async Task LoginAsync_EmailInexistente_DeberiaRetornarUnauthorized()
        {
            // Arrange
            _mockRepository.Setup(r => r.FindByEmailAsync("nadie@funko.com")).ReturnsAsync((UserEntity?)null);

            // Act
            var resultado = await _service.LoginAsync(new LoginDto("nadie@funko.com", "cualquiera"));

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.Unauthorized>();
        }

        [Test]
        public async Task LoginAsync_UsuarioBorrado_DeberiaRetornarUnauthorized()
        {
            // Arrange
            var usuario = CrearUsuario();
            usuario.IsDeleted = true;
            _mockRepository.Setup(r => r.FindByEmailAsync(usuario.Email)).ReturnsAsync(usuario);

            // Act
            var resultado = await _service.LoginAsync(new LoginDto(usuario.Email, "admin123"));

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().BeOfType<DomainError.Unauthorized>();
        }
    }
}
