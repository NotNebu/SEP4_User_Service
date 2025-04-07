using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

// Registrér afhængigheder til services og repositories i DI-containeren
builder.Services.AddScoped<IAuthService, AuthUserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Konfigurer Entity Framework Core til at bruge en in-memory database
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"));

var app = builder.Build();

// Konfigurer HTTP-pipelinen

// Map gRPC-tjenesten til AuthService
app.MapGrpcService<AuthGrpcService>();

// Basis GET-endpoint som informerer om gRPC-brug
app.MapGet("/", () =>
    "Communication with gRPC endpoints must be made through a gRPC client.");

// Map eventuelle API-kontrollere (hvis nødvendigt)
app.MapControllers();

// Start applikationen
app.Run();
