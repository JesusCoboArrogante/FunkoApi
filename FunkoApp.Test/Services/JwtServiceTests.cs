using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using FunkoAppi01.Config;
using FunkoAppi01.Entity;
using FunkoAppi01.Service;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace FunkoApp.Test.Services;

/// <summary>
/// Tests de JwtService.
/// </summary>
[TestFixture]
public class JwtServiceTests
{
    private JwtService _jwtService = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new JwtConfig
        {
            Secret = "esta-es-una-clave-de-prueba-de-al-menos-32-bytes",
            Issuer = "FunkoAppTests",
            Audience = "FunkoAppTestsClients",
            ExpirationMinutes = 15
        };

        _jwtService = new JwtService(Options.Create(config));
    }

    private static UserEntity CrearUsuario() => new()
    {
        Id = 1,
        Username = "admin",
        Email = "admin@funko.com",
        Role = UserRoles.Admin
    };

    [TestFixture]
    public class CasosPositivos : JwtServiceTests
    {
        [Test]
        public void GenerateToken_DeberiaIncluirLosClaimsDelUsuario()
        {
            // Arrange
            var usuario = CrearUsuario();

            // Act
            var token = _jwtService.GenerateToken(usuario);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // Assert
            jwt.Subject.Should().Be("1");
            jwt.Claims.Should().Contain(c => c.Type == "username" && c.Value == "admin");
            jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "ADMIN");
        }

        [Test]
        public void AccessTokenExpirySeconds_DeberiaConvertirLosMinutosASegundos()
        {
            // Assert
            _jwtService.AccessTokenExpirySeconds.Should().Be(15 * 60);
        }
    }
}
