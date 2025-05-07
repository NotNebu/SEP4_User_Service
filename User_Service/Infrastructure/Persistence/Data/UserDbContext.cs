using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Infrastructure.Persistence.Data;

// Databasekontekst for brugere og lokationer
public class UserDbContext : DbContext
{
    // Initialiserer databasekonteksten med konfigurationsmuligheder
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    // DbSet for brugere
    public DbSet<User> Users { get; set; }

    // Konfigurerer database-relationsmodeller
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfiguration for User-entiteten
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id); // Primærnøgle
            entity.Property(u => u.Id)
                  .ValueGeneratedOnAdd(); // Automatisk genereret ID
        });

        // Konfiguration for Location-entiteten
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasOne(l => l.User) // Relation til User
                  .WithMany(u => u.Locations) // En bruger kan have mange lokationer
                  .HasForeignKey(l => l.UserID) // Fremmednøgle
                  .OnDelete(DeleteBehavior.Cascade); // Sletning af bruger sletter lokationer
        });
    }
}
