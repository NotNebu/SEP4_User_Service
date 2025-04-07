using Application.Interfaces;
using Grpc.Core;
using UserService.Grpc;

public class AuthGrpcService : UserService.Grpc.AuthService.AuthServiceBase
{
    private readonly IAuthService _authService;

    public AuthGrpcService(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);
        return new LoginResponse { Token = token };
    }

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

    public override async Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
    {
        var user = await _authService.GetUserByTokenAsync(request.Token);
        if (user == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Ugyldig token"));

        return new UserResponse { Email = user.Email, Username = user.Username };
    }
}
