using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at håndtere login.
public class LoginUseCase : ILoginUseCase
{
    private readonly IAuthService _authService;

    // Initialiserer use case med en autentificeringstjeneste.
    public LoginUseCase(IAuthService authService)
    {
        _authService = authService;
    }

    // Udfører login-operationen.
    // Returnerer et JWT-token, hvis login lykkes.
  public async Task<string> ExecuteAsync(string email, string password)
        {
            var token = await _authService.LoginAsync(email, password);

            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Ugyldige loginoplysninger.");

            return token;
        }
}