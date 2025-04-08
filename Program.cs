using SEP4_User_Service.API.Services;
using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Infrastructure.Persistence.Data;
using SEP4_User_Service.Infrastructure.Persistence.Repositories;
using SEP4_User_Service.Application.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SEP4_User_Service.Application.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Sørg for at lytte på port 5001 på *alle* IP'er
    options.ListenAnyIP(5001, listenOptions =>
    {
        // Kør KUN HttpProtocols.Http2 for at tvinge ren h2c
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers();

// UseCases 
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<GetUserUseCase>();
builder.Services.AddScoped<GetAllUsersUseCase>();
builder.Services.AddScoped<UpdateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();

// PostgreSQL forbindelse
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Valider forbindelsen, hvis den er null eller tom, kast en undtagelse
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not provided or is empty.");
}

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcUserService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");
app.MapControllers();

app.Run();
