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

// Konfigurerer applikationen og dens services
var builder = WebApplication.CreateBuilder(args);

// Tilføjer miljøvariabler til konfiguration
builder.Configuration.AddEnvironmentVariables();

// Henter JWT-secret fra miljøvariabler
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Configuration["Jwt:Secret"] = jwtSecret;

// Konfigurerer JWT-baseret autentifikation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(jwtSecret!);
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

// Tilføjer middleware til autentifikation og autorisation
app.UseAuthentication();
app.UseAuthorization();

// Mapper gRPC-tjenester
app.MapGrpcService<AuthGrpcService>();
app.MapGrpcService<GrpcUserService>();

// Tilføjer et basis GET-endpoint
app.MapGet("/", () =>
    "Communication with gRPC endpoints must be made through a gRPC client.");

// Mapper API-kontrollere
app.MapControllers();

// Migrerer databasen ved opstart
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

// Starter applikationen
app.Run();
