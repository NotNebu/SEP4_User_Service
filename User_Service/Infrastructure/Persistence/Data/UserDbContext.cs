using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace SEP4_User_Service.Infrastructure.Persistence.Data
{
    // Databasekontekst for brugere, lokationer og eksperimenter
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Experiment> Experiments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguration for User-entiteten
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                      .ValueGeneratedOnAdd();

                // Auto-include af Locations relation, så den altid hentes med
                entity.Navigation(u => u.Locations)
                      .AutoInclude();

                // (Valgfrit) Auto-include eksperimenter
                // entity.Navigation(u => u.Experiments)
                //       .AutoInclude();
            });

            // Konfiguration for Location-entiteten
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.LocationID);
                entity.Property(l => l.LocationID)
                      .ValueGeneratedOnAdd();

                entity.HasOne(l => l.User)
                      .WithMany(u => u.Locations)
                      .HasForeignKey(l => l.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Konfiguration for Experiment-entiteten
            modelBuilder.Entity<Experiment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();

                entity.HasOne(e => e.User)
                      .WithMany(u => u.Experiments)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
