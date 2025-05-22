namespace SEP4_User_Service.Domain.Entities;

// Repræsenterer en lokation tilknyttet en bruger
public class Location
{
    public Guid LocationID { get; set; } // Lokationens unikke ID
    public string Street { get; set; } = string.Empty; // Gadenavn
    public string HouseNumber { get; set; } = string.Empty; // Husnummer
    public string City { get; set; } = string.Empty; // By
    public string Country { get; set; } = string.Empty; // Land

    // Fremmednøgle og navigationsegenskab til bruger
    public Guid UserID { get; set; }
    public User User { get; set; } = null!;

    // Konstruktor til at oprette en lokation med detaljer
    public Location(string street, string houseNumber, string city, string country)
    {
        Street = street;
        HouseNumber = houseNumber;
        City = city;
        Country = country;
    }

    // Parameterløs konstruktor til EF Core
    public Location() { }
}
