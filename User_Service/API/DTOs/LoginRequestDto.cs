using System.ComponentModel.DataAnnotations;

namespace SEP4_User_Service.API.DTOs
{
    // DTO til at håndtere loginanmodninger
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email er nødvendig.")]
        [EmailAddress(ErrorMessage = "Forkert email format.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password er nødvendig.")]
        public string Password { get; set; } = default!;
    }
}

