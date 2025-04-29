namespace SEP4_User_Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Domain.Entities;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<User>     Users     => Set<User>();
    public DbSet<Location> Locations => Set<Location>();
}
