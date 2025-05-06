using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces;

// Interface til brugerrepository.
// Definerer CRUD-operationer for brugerdata.
public interface IUserRepository
{
    // Opretter en ny bruger.
    Task CreateUserAsync(User user);

    // Henter en bruger baseret på ID.
    Task<User?> GetUserByIdAsync(Guid id);

    // Henter en bruger baseret på email.
    Task<User?> GetUserByEmailAsync(string email);

    // Opdaterer en brugers data.
    Task UpdateUserAsync(User user);

    // Sletter en bruger baseret på ID.
    Task<bool> DeleteUserAsync(Guid id);

    // Henter alle brugere.
    Task<List<User>> GetAllUsersAsync();

    // Henter brugere baseret på en liste af IDs.
    Task<List<User>> GetUsersByIdsAsync(List<Guid> ids);
}
