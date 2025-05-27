namespace SEP4_User_Service.API.DTOs;

using System.ComponentModel.DataAnnotations;

// DTO til at håndtere anmodninger om ændring af adgangskode
public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Gamle kodeord er nødvendig.")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Nyt kodeord er nødvendigt.")]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters.")]
    public string NewPassword { get; set; }
}
