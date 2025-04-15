using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByEmailAsync(string email);
    Task UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    Task<List<User>> GetAllUsersAsync();
    Task<List<User>> GetUsersByIdsAsync(List<Guid> ids);

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
