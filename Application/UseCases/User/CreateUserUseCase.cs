using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

public class CreateUserUseCase
{
    private readonly IUserRepository _repository;

    public CreateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> ExecuteAsync(User user)
    {
        await _repository.CreateUserAsync(user);
        return user;
    }
}
