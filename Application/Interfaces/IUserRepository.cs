using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByEmailAsync(string email);
    Task UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    Task<List<User>> GetAllUsersAsync();
    Task<List<User>> GetUsersByIdsAsync(List<Guid> ids);
    
}
