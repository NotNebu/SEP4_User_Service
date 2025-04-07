using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Interface til generering og validering af JWT-tokens for brugergodkendelse.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Genererer et JWT-token for en given bruger.
    /// </summary>
    /// <param name="user">Brugeren som tokenet skal oprettes for.</param>
    /// <returns>Et JWT-token som streng.</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Validerer et JWT-token og udtrækker brugerinformation.
    /// </summary>
    /// <param name="token">JWT-token der skal valideres.</param>
    /// <returns>Brugerobjektet hvis tokenet er gyldigt; ellers null.</returns>
    User? ValidateToken(string token);
}
