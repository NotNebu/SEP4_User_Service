using System;
using BCrypt.Net;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

/// <summary>
/// Implementering af autentificeringstjenesten, som håndterer login, registrering og validering af brugere.
/// </summary>
public class AuthUserService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;

    /// <summary>
    /// Initialiserer en ny instans af <see cref="AuthUserService"/>.
    /// </summary>
    /// <param name="userRepository">Repository til brugerdatabase.</param>
    /// <param name="tokenGenerator">Service til generering og validering af JWT-tokens.</param>
    public AuthUserService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    /// <summary>
    /// Logger en bruger ind ved at verificere email og adgangskode.
    /// </summary>
    /// <param name="email">Brugerens emailadresse.</param>
    /// <param name="password">Brugerens adgangskode.</param>
    /// <returns>Et JWT-token hvis login lykkes.</returns>
    /// <exception cref="UnauthorizedAccessException">Kastes hvis loginoplysningerne er ugyldige.</exception>
    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null)
            throw new UnauthorizedAccessException("Ugyldige legitimationsoplysninger");

        if (string.IsNullOrEmpty(user.Password))
            throw new UnauthorizedAccessException("Brugerens adgangskode er ugyldig");

        var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!passwordMatch)
            throw new UnauthorizedAccessException("Ugyldige legitimationsoplysninger");

        return _tokenGenerator.GenerateToken(user);
    }

    /// <summary>
    /// Registrerer en ny bruger med email, brugernavn og adgangskode.
    /// </summary>
    /// <param name="email">Emailadresse på den nye bruger.</param>
    /// <param name="password">Adgangskode som skal hashes og gemmes.</param>
    /// <param name="username">Brugerens ønskede brugernavn.</param>
    /// <returns>True hvis registreringen lykkes; ellers false hvis brugeren allerede findes.</returns>
    public async Task<bool> RegisterAsync(string email, string password, string username)
    {
        if (await _userRepository.GetUserByEmailAsync(email) != null)
            return false;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
        };

        await _userRepository.CreateUserAsync(user);
        return true;
    }

    /// <summary>
    /// Henter en bruger baseret på et JWT-token.
    /// </summary>
    /// <param name="token">JWT-token udstedt ved login.</param>
    /// <returns>Brugerobjektet hvis tokenet er gyldigt; ellers null.</returns>
    public async Task<User?> GetUserByTokenAsync(string token)
    {
        return _tokenGenerator.ValidateToken(token);
    }
}
