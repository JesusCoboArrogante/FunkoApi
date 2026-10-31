using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FunkoAppi01.Config;
using FunkoAppi01.Entity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FunkoAppi01.Service;

/// <summary>
/// Genera y valida los JWT de acceso.
/// </summary>
public class JwtService(IOptions<JwtConfig> config)
{
    private readonly JwtConfig _config = config.Value;

    /// <summary>Segundos de validez del access token.</summary>
    public int AccessTokenExpirySeconds => _config.ExpirationMinutes * 60;

    /// <summary>Genera un JWT firmado para el usuario dado.</summary>
    public string GenerateToken(UserEntity user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("username", user.Username),
            new("role", user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.ExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
