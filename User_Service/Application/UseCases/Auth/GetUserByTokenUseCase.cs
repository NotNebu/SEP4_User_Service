using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at hente en bruger baseret på et JWT-token.
public class GetUserByTokenUseCase : IGetUserByTokenUseCase
{
    private readonly IAuthService _authService;

    // Initialiserer use case med en autentificeringstjeneste.
    public GetUserByTokenUseCase(IAuthService authService)
    {
        _authService = authService;
    }

    // Henter en bruger baseret på token.
    // Returnerer brugerobjektet, hvis tokenet er gyldigt; ellers null.
    public Task<User?> ExecuteAsync(string token)
    {
        return _authService.GetUserByTokenAsync(token);
    }
}
