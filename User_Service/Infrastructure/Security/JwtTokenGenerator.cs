using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace Infrastructure.Security;

// Implementering af service til generering og validering af JWT-tokens
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;

    // Initialiserer JwtTokenGenerator med konfiguration
    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
    }

    // Genererer et JWT-token for en bruger
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

    // Validerer et JWT-token og returnerer den tilknyttede bruger
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
