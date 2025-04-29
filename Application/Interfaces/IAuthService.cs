using SEP4_User_Service.Domain.Entities;

/// <summary>
/// Interface for autentificeringstjenester, herunder login, registrering og hentning af brugerdata.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Logger en bruger ind baseret på email og adgangskode.
    /// </summary>
    /// <param name="email">Brugerens emailadresse.</param>
    /// <param name="password">Brugerens adgangskode.</param>
    /// <returns>Et JWT-token som streng, hvis login er succesfuldt.</returns>
    Task<string> LoginAsync(string email, string password);

    /// <summary>
    /// Registrerer en ny bruger med email, adgangskode og brugernavn.
    /// </summary>
    /// <param name="email">Brugerens emailadresse.</param>
    /// <param name="password">Brugerens ønskede adgangskode.</param>
    /// <param name="username">Brugerens ønskede brugernavn.</param>
    /// <returns>True hvis registreringen lykkedes, ellers false.</returns>
    Task<bool> RegisterAsync(string email, string password, string username);

    /// <summary>
    /// Henter en bruger baseret på et JWT-token.
    /// </summary>
    /// <param name="token">JWT-token, der identificerer brugeren.</param>
    /// <returns>Brugerobjektet, hvis tokenet er gyldigt; ellers null.</returns>
    Task<User?> GetUserByTokenAsync(string token);
}
