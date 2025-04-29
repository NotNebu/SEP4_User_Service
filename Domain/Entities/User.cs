namespace SEP4_User_Service.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Username  { get; set; } = string.Empty;
    public string Password  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Firstname { get; set; } = string.Empty;
    public string Lastname  { get; set; } = string.Empty;
    public string Birthday  { get; set; } = string.Empty;

    public ICollection<Location> Locations { get; set; } = new List<Location>();

    public User(
        string username,
        string password,
        string email,
        string firstname,
        string lastname,
        string birthday,
        ICollection<Location>? locations = null)
    {
        Id        = Guid.NewGuid();
        Username  = username;
        Password  = password;
        Email     = email;
        Firstname = firstname;
        Lastname  = lastname;
        Birthday  = birthday;
        Locations = locations ?? new List<Location>();
    }

    // Parameter­løs konstruktor til EF Core
    public User() { }
}
