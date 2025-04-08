using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository-implementering for brugere, som interagerer med databasen via Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    /// <summary>
    /// Initialiserer en ny instans af <see cref="UserRepository"/> med en given databasekontekst.
    /// </summary>
    /// <param name="context">Databasekontekst for brugere.</param>
    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Henter en bruger baseret på email.
    /// </summary>
    /// <param name="email">Emailadressen der skal søges på.</param>
    /// <returns>Brugerobjektet hvis det findes; ellers null.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Tilføjer en ny bruger til databasen og gemmer ændringerne.
    /// </summary>
    /// <param name="user">Brugerobjektet der skal tilføjes.</param>
    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Tjekker om en bruger med den givne email allerede findes i databasen.
    /// </summary>
    /// <param name="email">Emailadressen der skal tjekkes.</param>
    /// <returns>True hvis brugeren findes; ellers false.</returns>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
