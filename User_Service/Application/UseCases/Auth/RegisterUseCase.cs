using System;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;
using System.Threading.Tasks;

namespace SEP4_User_Service.Application.UseCases
{
    public class RegisterUseCase : IRegisterUseCase
    {
        private readonly IUserRepository _userRepository;

        public RegisterUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ExecuteAsync(string email, string password, string username)
        {
            Console.WriteLine($"Registreringsforsøg for: {email}");

            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                Console.WriteLine("Bruger eksisterer allerede.");
                return false;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
            };

            Console.WriteLine("Bruger oprettes...");
            await _userRepository.CreateUserAsync(user);

            Console.WriteLine("Bruger oprettet.");
            return true;
        }
    }
}

