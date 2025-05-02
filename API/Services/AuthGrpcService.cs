using SEP4_User_Service.Application.Interfaces;
using Grpc.Core;
using UserService.Grpc;

/// <summary>
/// gRPC-implementering af AuthService, som håndterer login, registrering og hentning af brugeroplysninger.
/// </summary>
public class AuthGrpcService : UserService.Grpc.AuthService.AuthServiceBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initialiserer en ny instans af AuthGrpcService med den underliggende autentificeringsservice.
    /// </summary>
    /// <param name="authService">Service der håndterer login, registrering og brugervalidering.</param>
    public AuthGrpcService(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Logger en bruger ind baseret på email og adgangskode.
    /// </summary>
    /// <param name="request">Login-forespørgsel med email og adgangskode.</param>
    /// <param name="context">gRPC-kontekst.</param>
    /// <returns>Et JWT-token, hvis login er succesfuldt.</returns>
    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);
        return new LoginResponse { Token = token };
    }

    /// <summary>
    /// Registrerer en ny bruger med email, adgangskode og brugernavn.
    /// </summary>
    /// <param name="request">Registreringsdata.</param>
    /// <param name="context">gRPC-kontekst.</param>
    /// <returns>En succesindikator.</returns>
    public override async Task<RegisterResponse> Register(
        RegisterRequest request,
        ServerCallContext context
    )
    {
        var success = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.Username
        );
        return new RegisterResponse { Success = success };
    }

    /// <summary>
    /// Henter brugerinformation baseret på et JWT-token.
    /// </summary>
    /// <param name="request">Forespørgsel med token.</param>
    /// <param name="context">gRPC-kontekst.</param>
    /// <returns>Brugerens email og brugernavn, hvis token er gyldigt.</returns>
    /// <exception cref="RpcException">Kastes hvis tokenet er ugyldigt.</exception>
    public override async Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
    {
        var user = await _authService.GetUserByTokenAsync(request.Token);
        if (user == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Ugyldig token"));

        return new UserResponse { Email = user.Email, Username = user.Username };
    }
}
