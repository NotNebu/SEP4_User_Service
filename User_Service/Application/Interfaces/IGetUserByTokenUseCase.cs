using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces
{
    // Interface til use case for at hente en bruger baseret på et JWT-token
    public interface IGetUserByTokenUseCase
    {
        // Henter en bruger baseret på token
        // Returnerer brugerobjektet, hvis tokenet er gyldigt; ellers null
        Task<User?> ExecuteAsync(string token);
    }
}