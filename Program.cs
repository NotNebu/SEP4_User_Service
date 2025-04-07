using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Sørg for at lytte på port 5001 på *alle* IP'er
    options.ListenAnyIP(
        5001,
        listenOptions =>
        {
            // Kør KUN HttpProtocols.Http2 for at tvinge ren h2c
            listenOptions.Protocols = HttpProtocols.Http2;
        }
    );
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers();

// Registrer interfaces
builder.Services.AddScoped<IAuthService, AuthUserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Database context
builder.Services.AddDbContext<UserDbContext>(options => options.UseInMemoryDatabase("UserDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuthGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
app.MapControllers();

app.Run();
