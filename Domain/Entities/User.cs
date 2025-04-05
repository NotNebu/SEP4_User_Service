using Microsoft.AspNetCore.Components.Routing;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public String Username { get; set; } = String.Empty;
    public String Password { get; set; } = String.Empty;
    public String Email { get; set; } = String.Empty;
    public String Firstname { get; set; } = String.Empty;
    public string Lastname { get; set; } = String.Empty;
    public DateTime Birthday { get; set; }
    public DateTime CreatedProfileDate { get; set; } = DateTime.UtcNow;
    public DateTime? LoginTime { get; set; }
    public ICollection<Location> Locations { get; set; } = new List<Location>();

    public User(string username, string password, string email, string firstname, string lastname, DateTime birthday, ICollection<Location>? locations = null)
    {
        Username = username;
        Password = password;
        Email = email;
        Firstname = firstname;
        Lastname = lastname;
        Birthday = birthday;
        Locations = locations ?? new List<Location>();
    }

    public User(){
        
    }
}