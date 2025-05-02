using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces
{
    public interface IGetUserByTokenUseCase
    {
        Task<User?> ExecuteAsync(string token);
    }
}