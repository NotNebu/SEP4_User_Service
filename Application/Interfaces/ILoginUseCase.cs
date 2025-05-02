namespace SEP4_User_Service.Application.Interfaces
{
    public interface ILoginUseCase
    {
        Task<string> ExecuteAsync(string email, string password);
    }
}