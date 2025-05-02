using SEP4_User_Service.Application.Interfaces;
using Grpc.Core;
using UserService.Grpc;

// gRPC-tjeneste til autentificering
public class AuthGrpcService : UserService.Grpc.AuthService.AuthServiceBase
{
    private readonly IAuthService _authService;

    // Initialiserer tjenesten med en autentificeringsservice
    public AuthGrpcService(IAuthService authService)
    {
        _authService = authService;
    }

    // Håndterer login og returnerer et JWT-token
    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);
        return new LoginResponse { Token = token };
    }

    // Registrerer en ny bruger
    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var success = await _authService.RegisterAsync(request.Email, request.Password, request.Username);
        return new RegisterResponse { Success = success };
    }

    // Henter brugeroplysninger baseret på et JWT-token
    public override async Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
    {
        var user = await _authService.GetUserByTokenAsync(request.Token);
        if (user == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Ugyldig token"));

        return new UserResponse { Email = user.Email, Username = user.Username };
    }
}
