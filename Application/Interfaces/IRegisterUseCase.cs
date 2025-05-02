namespace SEP4_User_Service.Application.Interfaces
{
    public interface IRegisterUseCase
    {
        Task<bool> ExecuteAsync(string email, string password, string username);
    }
}