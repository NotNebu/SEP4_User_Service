namespace SEP4_User_Service.Application.Interfaces
{
    // Interface til registrerings-use case.
    // Definerer en metode til at registrere en ny bruger.
    public interface IRegisterUseCase
    {
        // Registrerer en ny bruger med email, adgangskode og brugernavn.
        // Returnerer true, hvis registreringen lykkes; ellers false.
        Task<bool> ExecuteAsync(string email, string password, string username);
    }
}