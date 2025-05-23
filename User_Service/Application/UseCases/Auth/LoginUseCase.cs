using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;
using System;

namespace SEP4_User_Service.Application.UseCases
{
    public class LoginUseCase : ILoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public LoginUseCase(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> ExecuteAsync(string email, string password)
        {
            Console.WriteLine($"Login attempt for: {email}");

            var user = await _userRepository.GetUserByEmailAsync(email);

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
    }
}
