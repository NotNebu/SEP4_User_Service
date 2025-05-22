using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases
{
    // Use case til at ændre en brugers adgangskode
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly IUserRepository _userRepository;

        // Initialiserer use case med et brugerrepository
        public ChangePasswordUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Ændrer en brugers adgangskode
        public async Task<bool> ExecuteAsync(Guid userId, string oldPassword, string newPassword)
        {
            // Henter brugeren baseret på ID
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            // Verificerer det gamle kodeord
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                return false;

            // Opdaterer adgangskoden med en ny hash
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateUserAsync(user);

            return true;
        }
    }
}
