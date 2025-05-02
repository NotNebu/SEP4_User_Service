using System;
using System.Threading.Tasks;

namespace SEP4_User_Service.Application.Interfaces
{
    public interface IChangePasswordUseCase
    {
        Task<bool> ExecuteAsync(Guid userId, string oldPassword, string newPassword);
    }
}
