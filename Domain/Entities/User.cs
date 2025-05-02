namespace SEP4_User_Service.Domain.Entities;

// Repræsenterer en bruger i systemet
public class User
{
    public Guid Id { get; set; } // Brugerens unikke ID
    public string Username { get; set; } = string.Empty; // Brugernavn
    public string Password { get; set; } = string.Empty; // Adgangskode
    public string Email { get; set; } = string.Empty; // Emailadresse
    public string Firstname { get; set; } = string.Empty; // Fornavn
    public string Lastname { get; set; } = string.Empty; // Efternavn
    public string Birthday { get; set; } = string.Empty; // Fødselsdato

    // Liste over lokationer tilknyttet brugeren
    public ICollection<Location> Locations { get; set; } = new List<Location>();

    // Konstruktor til at oprette en bruger med detaljer
    public User(string username, string password, string email, string firstname, string lastname, string birthday, ICollection<Location>? locations = null)
    {
        Id = Guid.NewGuid();
        Username = username;
        Password = password;
        Email = email;
        Firstname = firstname;
        Lastname = lastname;
        Birthday = birthday;
        Locations = locations ?? new List<Location>();
    }

    // Parameterløs konstruktor til EF Core
    public User() { }
}
