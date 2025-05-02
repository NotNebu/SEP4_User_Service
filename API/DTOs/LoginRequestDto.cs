namespace SEP4_User_Service.API.DTOs;

    public class LoginRequestDto
    {
        /// <summary>
        /// Brugerens emailadresse.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// Brugerens adgangskode i klartekst (skal sendes over HTTPS).
        /// </summary>
        public string Password { get; set; } = default!;
    }

