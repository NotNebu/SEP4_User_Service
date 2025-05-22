using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SEP4_User_Service.Infrastructure.Persistence.Data; 

// Denne factory bruges kun når man kører EF-kommandoer som fx 'dotnet ef migrations update'.
// Den her bruges altså kun af EF CLI og ikke når appen kører i Docker eller i produktion.
    
public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{       
    public UserDbContext CreateDbContext(string[] args)
    {       
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=userdb;Username=sep4;Password=sep4");

        return new UserDbContext(optionsBuilder.Options);
    }
}

