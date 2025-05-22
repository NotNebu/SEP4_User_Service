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
using System.Threading.Tasks;

// Denne fil konfigurerer og starter applikationen.
var builder = WebApplication.CreateBuilder(args);

// Tilføjer miljøvariabler til konfiguration.
builder.Configuration.AddEnvironmentVariables();

// Henter JWT-secret fra miljøvariabler og konfigurerer JWT-baseret autentifikation.
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

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Debug logs fjernet
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var cookieToken = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    // Debug logs fjernet
                    context.Token = cookieToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Konfigurerer Kestrel til at lytte på port 5001 og understøtte HTTPS og HTTP/2.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps("/certs/localhost-user-service.p12", "changeit");
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

// Tilføjer gRPC og API-kontroller til DI-containeren.
builder.Services.AddGrpc();
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
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// Bygger applikationen.
var app = builder.Build();

// Tilføjer middleware til autentifikation og autorisation.
app.UseAuthentication();
app.UseAuthorization();

// Mapper gRPC-tjenester og API-kontrollere.
app.MapGrpcService<AuthGrpcService>();
app.MapGrpcService<GrpcUserService>();
app.MapGrpcService<GrpcExperimentService>();
app.MapGrpcService<GrpcPredictionService>();
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
