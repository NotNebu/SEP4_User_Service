using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.Exceptions;

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
    // Kaster exception hvis brugeren allerede findes.
public async Task<bool> ExecuteAsync(string email, string password, string username)
        {
            var success = await _authService.RegisterAsync(email, password, username);

            if (!success)
            {
                throw new UserAlreadyExistsException("Bruger med denne email findes allerede.");
            }

            return true;
        }
    }