using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces;

/// <summary>
/// Interface for autentificeringstjenester, herunder login, registrering og hentning af brugerdata.
/// </summary>
public interface IAuthService
{
    Task<string> LoginAsync(string email, string password);

    Task<bool> RegisterAsync(string email, string password, string username);

    Task<User?> GetUserByTokenAsync(string token);
}
