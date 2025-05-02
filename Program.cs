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

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Configuration["Jwt:Secret"] = jwtSecret;

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

builder.Services.AddAuthorization();

// Konfigurer Kestrel til kun at bruge HTTP/2 (krævet for gRPC uden TLS - h2c)
builder.WebHost.ConfigureKestrel(options =>
{
    // Lyt på port 5001 på alle IP-adresser
    options.ListenAnyIP(
        5001,
        listenOptions =>
        {
            // Brug kun HTTP/2 (uden HTTP/1.1 fallback)
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
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
builder.Services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();

// Konfigurer Entity Framework Core til at bruge en in-memory database
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Konfigurer Middleware
app.UseAuthentication();
app.UseAuthorization();

// Map gRPC-tjenesten til AuthService
app.MapGrpcService<AuthGrpcService>();
app.MapGrpcService<GrpcUserService>();

// Basis GET-endpoint som informerer om gRPC-brug
app.MapGet("/", () =>
    "Communication with gRPC endpoints must be made through a gRPC client.");

// Map eventuelle API-kontrollere
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

// Start applikationen
app.Run();
