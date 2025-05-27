using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);

    User? ValidateToken(string token);
}
