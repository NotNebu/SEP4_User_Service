namespace SEP4_User_Service.API.DTOs;

// DTO til at repræsentere en bruger uden følsomme oplysninger
public class UserDto
{
    public string Email { get; set; } = default!; // Brugerens emailadresse
    public string Username { get; set; } = default!; // Brugerens brugernavn
}


