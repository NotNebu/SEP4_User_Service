using System;
using System.Threading.Tasks;

namespace SEP4_User_Service.Application.Interfaces
{
    // Interface til use case for at ændre en brugers adgangskode
    public interface IChangePasswordUseCase
    {
        // Ændrer en brugers adgangskode
        // Returnerer true, hvis ændringen lykkes; ellers false
        Task<bool> ExecuteAsync(Guid userId, string oldPassword, string newPassword);
    }
}
