using System;
using Application.Interfaces;
using Domain.Entities;

public class AuthUserService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthUserService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        Console.WriteLine($"Login attempt for: {email}");

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            Console.WriteLine("Bruger ikke fundet.");
            throw new UnauthorizedAccessException("Ugyldige legitimationsoplysninger");
        }

        Console.WriteLine($"Bruger fundet: {user.Email}");

        if (string.IsNullOrEmpty(user.Password))
        {
            Console.WriteLine("Brugerens password er null eller tomt!");
        }

        var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
        Console.WriteLine($"Password match: {passwordMatch}");

        if (!passwordMatch)
            throw new UnauthorizedAccessException("Ugyldige legitimationsoplysninger");

        var token = _tokenGenerator.GenerateToken(user);
        Console.WriteLine("Token genereret");

        return token;
    }

    public async Task<bool> RegisterAsync(string email, string password, string username)
    {
        if (await _userRepository.ExistsByEmailAsync(email))
            return false;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
        };

        await _userRepository.AddAsync(user);
        return true;
    }

    public async Task<User?> GetUserByTokenAsync(string token)
    {
        return _tokenGenerator.ValidateToken(token);
    }
}
