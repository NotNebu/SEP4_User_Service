namespace SEP4_User_Service.Application.Interfaces
{
    // Interface til login-use case.
    // Definerer en metode til at udføre login-operationen.
    public interface ILoginUseCase
    {
        // Udfører login ved at verificere brugerens email og adgangskode.
        // Returnerer et JWT-token, hvis login lykkes.
        Task<string> ExecuteAsync(string email, string password);
    }
}
