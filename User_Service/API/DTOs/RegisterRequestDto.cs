using System.ComponentModel.DataAnnotations;

namespace SEP4_User_Service.API.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email er nødvendig.")]
        [EmailAddress(ErrorMessage = "Forkert email format.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Kodeord er nødvendig.")]
        [PasswordStrength]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Brugernavn er nødvendig.")]
        [MinLength(3, ErrorMessage = "Brugernavn skal være mindst 3 karakterer lang.")]
        public string Username { get; set; } = default!;
    }
}
