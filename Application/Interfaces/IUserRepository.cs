using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Interface for adgang til og håndtering af brugerdata i datalaget.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Henter en bruger baseret på email.
    /// </summary>
    /// <param name="email">Emailadressen på den bruger, der ønskes hentet.</param>
    /// <returns>Brugerobjektet hvis det findes; ellers null.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Tjekker om en bruger med den givne email allerede findes.
    /// </summary>
    /// <param name="email">Emailadressen der skal tjekkes.</param>
    /// <returns>True hvis brugeren findes; ellers false.</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Tilføjer en ny bruger til databasen.
    /// </summary>
    /// <param name="user">Brugerobjektet der skal tilføjes.</param>
    /// <returns>Et Task-objekt der repræsenterer den asynkrone operation.</returns>
    Task AddAsync(User user);
}
