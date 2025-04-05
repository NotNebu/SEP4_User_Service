using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    User? ValidateToken(string token);
}
