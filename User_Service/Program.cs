using SEP4_User_Service.API.Services;
using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Infrastructure.Persistence.Repositories;
using SEP4_User_Service.Application.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Infrastructure.Persistence.Data;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Grpc.AspNetCore.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

// Konfigurerer applikationen og dens services
var builder = WebApplication.CreateBuilder(args);

// Tilføjer miljøvariabler til konfiguration
builder.Configuration.AddEnvironmentVariables();

// Henter JWT-secret fra miljøvariabler
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    jwtSecret = "InMemoryTestingSecretDontUseInProduction";
}
builder.Configuration["Jwt:Secret"] = jwtSecret;

// Konfigurer JWT‐autentifikation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // NU er jwtSecret altid en string, ikke null
        var key = Encoding.UTF8.GetBytes(jwtSecret);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// Tilføjer autorisation
builder.Services.AddAuthorization();

// Konfigurerer Kestrel til at lytte på port 5001 og understøtte HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

// Tilføjer gRPC og API-kontroller til DI-containeren
builder.Services.AddGrpc(); 
builder.Services.AddControllers();

// Registrerer UseCases og services i DI-containeren
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<GetUserUseCase>();
builder.Services.AddScoped<GetAllUsersUseCase>();
builder.Services.AddScoped<UpdateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();
builder.Services.AddScoped<IAuthService, AuthUserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();

// Konfigurerer Entity Framework Core til at bruge PostgreSQL
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// Bygger applikationen
var app = builder.Build();

// grpc-web middleware (DefaultEnabled = true betyder at alle services understøtter grpc-web uden at du behøver .EnableGrpcWeb() på hver enkelt)
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Tilføjer middleware til autentifikation og autorisation
app.UseAuthentication();
app.UseAuthorization();


// Mapper gRPC-tjenester
app.MapGrpcService<AuthGrpcService>()
   .EnableGrpcWeb();    // <-- gør Auth gRPC tilgængelig via grpc-web
app.MapGrpcService<GrpcUserService>()
   .EnableGrpcWeb();  

// Tilføjer et basis GET-endpoint
app.MapGet("/", () =>
    "Communication with gRPC endpoints must be made through a gRPC client.");

// Mapper API-kontrollere
app.MapControllers();

// Migrerer kun databasen, hvis vi IKKE kører i “Testing” environment
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}


// Starter applikationen
app.Run();


// Til test: Stub-klasse så WebApplicationFactory<Program> kan finde entry-point
namespace SEP4_User_Service.API
{
    public partial class Program { }
}
