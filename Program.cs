using SEP4_User_Service.API.Services;
using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Infrastructure.Persistence.Repositories;
using SEP4_User_Service.Application.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SEP4_User_Service.Application.UseCases;
using Application.Interfaces;
using SEP4_User_Service.Infrastructure.Persistence;
using Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Configuration["Jwt:Secret"] = jwtSecret;

// Konfigurer Kestrel til kun at bruge HTTP/2 (krævet for gRPC uden TLS - h2c)
builder.WebHost.ConfigureKestrel(options =>
{
    // Lyt på port 5001 på alle IP-adresser
    options.ListenAnyIP(
        5001,
        listenOptions =>
        {
            // Brug kun HTTP/2 (uden HTTP/1.1 fallback)
            listenOptions.Protocols = HttpProtocols.Http2;
        }
    );
});

// Tilføj gRPC og MVC/Controller-support til DI-containeren
builder.Services.AddGrpc();
builder.Services.AddControllers();

// UseCases 
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<GetUserUseCase>();
builder.Services.AddScoped<GetAllUsersUseCase>();
builder.Services.AddScoped<UpdateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();
builder.Services.AddScoped<IAuthService, AuthUserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Konfigurer Entity Framework Core til at bruge en in-memory database
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Konfigurer HTTP-pipelinen

// Map gRPC-tjenesten til AuthService
app.MapGrpcService<AuthGrpcService>();
app.MapGrpcService<GrpcUserService>();

// Basis GET-endpoint som informerer om gRPC-brug
app.MapGet("/", () =>
    "Communication with gRPC endpoints must be made through a gRPC client.");

// Map eventuelle API-kontrollere
app.MapControllers();

// Start applikationen
app.Run();
