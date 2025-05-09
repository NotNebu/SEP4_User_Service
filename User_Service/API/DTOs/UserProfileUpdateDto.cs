// Denne klasse repræsenterer dataoverførselsobjektet (DTO) for opdatering af en brugers profil.
// DTO bruges til at modtage opdaterede brugerprofildata fra klienten.
namespace SEP4_User_Service.API.DTOs;

public class UserProfileUpdateDto
{
    // Brugerens fornavn.
    public string Firstname { get; set; } = string.Empty;

    // Brugerens efternavn.
    public string Lastname { get; set; } = string.Empty;

    // Brugerens brugernavn.
    public string Username { get; set; } = string.Empty;

    // Brugerens e-mailadresse.
    public string Email { get; set; } = string.Empty;

    // Brugerens fødselsdato.
    public string Birthday { get; set; } = string.Empty;

    // Brugerens land.
    public string Country { get; set; } = string.Empty;

    // Brugerens gadeadresse.
    public string Street { get; set; } = string.Empty;

    // Brugerens husnummer.
    public string HouseNumber { get; set; } = string.Empty;

    // Brugerens by.
    public string City { get; set; } = string.Empty;
}
