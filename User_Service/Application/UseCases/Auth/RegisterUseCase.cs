using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at håndtere brugerregistrering.
public class RegisterUseCase : IRegisterUseCase
{
    private readonly IAuthService _authService;

    // Initialiserer use case med en autentificeringstjeneste.
    public RegisterUseCase(IAuthService authService)
    {
        _authService = authService;
    }

    // Registrerer en ny bruger med email, adgangskode og brugernavn.
    // Returnerer true, hvis registreringen lykkes; ellers false.
    public Task<bool> ExecuteAsync(string email, string password, string username)
    {
        return _authService.RegisterAsync(email, password, username);
    }
}
