using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

/// <summary>
/// Implementering af <see cref="IJwtTokenGenerator"/> til generering og validering af JWT-tokens.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Initialiserer en ny instans af <see cref="JwtTokenGenerator"/> med konfigurationen.
    /// </summary>
    /// <param name="config">Konfigurationsobjekt, typisk med adgang til JWT-secret.</param>
    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Genererer et JWT-token baseret på en bruger.
    /// </summary>
    /// <param name="user">Brugeren som tokenet skal oprettes for.</param>
    /// <returns>Et signeret JWT-token som streng.</returns>
    public string GenerateToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]);
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.Id.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Validerer et JWT-token og returnerer den tilknyttede bruger, hvis gyldig.
    /// </summary>
    /// <param name="token">JWT-token der skal valideres.</param>
    /// <returns>Et <see cref="User"/>-objekt hvis tokenet er gyldigt; ellers null.</returns>
    public User? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                },
                out SecurityToken validatedToken
            );

            var jwtToken = (JwtSecurityToken)validatedToken;
            var email = jwtToken.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var id = Guid.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

            return new User { Id = id, Email = email };
        }
        catch
        {
            return null;
        }
    }
}
