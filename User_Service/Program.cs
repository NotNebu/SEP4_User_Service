using System.Text;
using System.Threading.Tasks;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Application.UseCases;
using SEP4_User_Service.Infrastructure.Persistence.Data;
using SEP4_User_Service.Infrastructure.Persistence.Repositories;

// Denne fil konfigurerer og starter applikationen.
var builder = WebApplication.CreateBuilder(args);

// Tilføjer miljøvariabler til konfiguration.
builder.Configuration.AddEnvironmentVariables();

// Henter JWT-secret fra miljøvariabler og konfigurerer JWT-baseret autentifikation.
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
builder.Configuration["Jwt:Secret"] = jwtSecret;

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            ClockSkew = TimeSpan.Zero,
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var cookieToken = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                }
                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddAuthorization();

// Konfigurerer Kestrel til at lytte på port 5001 og understøtte HTTPS og HTTP/2.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(
        5001,
        listenOptions =>
        {
            listenOptions.UseHttps("/certs/localhost-user-service.p12", "changeit");
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        }
    );
});

// Tilføjer API-kontroller til DI-containeren.
builder.Services.AddControllers();

// Registrerer UseCases og services i DI-containeren.
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<GetUserUseCase>();
builder.Services.AddScoped<GetAllUsersUseCase>();
builder.Services.AddScoped<UpdateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();
builder.Services.AddScoped<CreateExperimentUseCase>();
builder.Services.AddScoped<UpdateExperimentUseCase>();
builder.Services.AddScoped<DeleteExperimentUseCase>();
builder.Services.AddScoped<GetExperimentByIdUseCase>();
builder.Services.AddScoped<IRegisterUseCase, RegisterUseCase>();
builder.Services.AddScoped<IGetUserByTokenUseCase, GetUserByTokenUseCase>();
builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
builder.Services.AddScoped<IAuthService, AuthUserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
builder.Services.AddScoped<IExperimentRepository, ExperimentRepository>();
builder.Services.AddScoped<IPredictionRepository, PredictionRepository>();

// Konfigurerer Entity Framework Core til at bruge PostgreSQL.
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(connectionString));

// Bygger applikationen.
var app = builder.Build();

// Tilføjer middleware til autentifikation og autorisation.
app.UseAuthentication();
app.UseAuthorization();

// Mapper API-kontrollere.
app.MapControllers();

// Migrerer databasen ved opstart.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

// Starter applikationen.
app.Run();

// Til test: Stub-klasse så WebApplicationFactory<Program> kan finde entry-point
namespace SEP4_User_Service.API
{
    public partial class Program { }
}
