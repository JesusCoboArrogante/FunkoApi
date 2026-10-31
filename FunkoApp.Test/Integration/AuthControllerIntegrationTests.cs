using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FunkoAppi01.Dto;
using NUnit.Framework;

namespace FunkoApp.Test.Integration;

/// <summary>
/// Tests de integración de AuthController: servidor en memoria con
/// WebApplicationFactory, contra los usuarios semilla reales.
/// </summary>
[TestFixture]
public class AuthControllerIntegrationTests
{
    private FunkoAppWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new FunkoAppWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [TestFixture]
    public class CasosPositivos : AuthControllerIntegrationTests
    {
        [Test]
        public async Task Login_UsuarioSemillaConCredencialesCorrectas_DeberiaDevolverToken()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginDto("admin@funko.com", "admin123"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            body!.AccessToken.Should().NotBeNullOrEmpty();
            body.Role.Should().Be("ADMIN");
        }

        [Test]
        public async Task Signup_EmailNuevo_DeberiaDevolverCreated()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/signup",
                new SignupDto("cristian", "cristian@funko.com", "MiClave123"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task Me_ConTokenValido_DeberiaDevolverLosClaims()
        {
            // Arrange
            var login = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginDto("admin@funko.com", "admin123"));
            var token = (await login.Content.ReadFromJsonAsync<AuthResponseDto>())!.AccessToken;
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/auth/me");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [TestFixture]
    public class CasosNegativos : AuthControllerIntegrationTests
    {
        [Test]
        public async Task Login_PasswordIncorrecta_Devuelve401()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginDto("admin@funko.com", "password-mala"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Signup_EmailYaRegistrado_Devuelve409()
        {
            // Act: admin@funko.com ya existe por el seed
            var response = await _client.PostAsJsonAsync("/api/auth/signup",
                new SignupDto("otro", "admin@funko.com", "MiClave123"));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Test]
        public async Task Me_SinToken_Devuelve401()
        {
            // Act
            var response = await _client.GetAsync("/api/auth/me");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
