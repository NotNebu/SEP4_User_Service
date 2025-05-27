using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.Infrastructure.Persistence.Data;

namespace SEP4_User_Service.Infrastructure.Persistence.Repositories;

// Implementering af brugerrepository til databaseoperationer
public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    // Initialiserer repository med databasekontekst
    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    // Tilføjer en ny bruger til databasen
    public async Task CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    // Henter en bruger baseret på ID
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    // Henter en bruger baseret på email
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    // Opdaterer en brugers data
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    // Sletter en bruger baseret på ID
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // Henter alle brugere fra databasen
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    // Henter brugere baseret på en liste af IDs
    public async Task<List<User>> GetUsersByIdsAsync(List<Guid> ids)
    {
        return await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
    }
}
