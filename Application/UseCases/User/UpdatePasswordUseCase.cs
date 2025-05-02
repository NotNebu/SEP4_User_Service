using System;
using System.Threading.Tasks;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases
{
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly IUserRepository _userRepository;

        public ChangePasswordUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ExecuteAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateUserAsync(user);

            return true;
        }
    }
}
