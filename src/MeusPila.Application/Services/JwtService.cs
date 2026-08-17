using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MeusPila.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserEntity = MeusPila.Domain.Entities.User;

namespace MeusPila.Application.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        string? secretKey = _configuration.GetValue<string>("Jwt:SecretKey");
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("A chave secreta do JWT não está configurada.");
        }

        string? issuer = _configuration.GetValue<string>("Jwt:Issuer");
        if (string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException("O emissor do JWT não está configurado.");
        }

        string? audience = _configuration.GetValue<string>("Jwt:Audience");
        if (string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("O público do JWT não está configurado.");
        }

        double? expiresInHours = _configuration.GetValue<double?>("Jwt:ExpirationTimeH");
        if (!expiresInHours.HasValue)
        {
            expiresInHours = 1; // Default to 1 hour if not configured
        }
        else
        {
            if (expiresInHours <= 0)
            {
                throw new InvalidOperationException("O tempo de expiração do JWT deve ser maior que zero.");
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiresInHours.Value),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
