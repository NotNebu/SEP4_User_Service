using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Databasens kontekstklasse for brugere, som anvender Entity Framework Core.
/// </summary>
public class UserDbContext : DbContext
{
    /// <summary>
    /// Initialiserer en ny instans af <see cref="UserDbContext"/> med de angivne konfigurationsmuligheder.
    /// </summary>
    /// <param name="options">Options til konfiguration af DbContext.</param>
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    /// <summary>
    /// Repræsenterer tabellen over brugere i databasen.
    /// </summary>
    public DbSet<User> Users { get; set; }
}
