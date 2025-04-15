using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Infrastructure.Persistence.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Id)
              .ValueGeneratedOnAdd(); 
    });

     modelBuilder.Entity<Location>(entity =>
    {
         entity.HasOne(l => l.User)
              .WithMany(u => u.Locations)
              .HasForeignKey(l => l.UserID)
              .OnDelete(DeleteBehavior.Cascade);
    });

}
}
