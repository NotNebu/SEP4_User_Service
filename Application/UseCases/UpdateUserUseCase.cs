using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

public class UpdateUserUseCase
{
    private readonly IUserRepository _repository;

    public UpdateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(User user)
    {
        await _repository.UpdateUserAsync(user);
    }
}
