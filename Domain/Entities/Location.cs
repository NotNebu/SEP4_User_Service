namespace SEP4_User_Service.Domain.Entities;

public class Location
{
    public Guid   LocationID  { get; set; }

    public string Street      { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string City        { get; set; } = string.Empty;
    public string Country     { get; set; } = string.Empty;

    // FK + navigation til User
    public Guid UserID { get; set; }
    public User User   { get; set; } = null!;

    public Location(
        string street,
        string houseNumber,
        string city,
        string country)
    {
        Street      = street;
        HouseNumber = houseNumber;
        City        = city;
        Country     = country;
    }

    // Parameterløs konstruktor til EF Core
    public Location() { }
}
