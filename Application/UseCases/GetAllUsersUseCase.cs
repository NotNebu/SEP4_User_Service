using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

public class GetAllUsersUseCase
{
    private readonly IUserRepository _repository;

    public GetAllUsersUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<User>> ExecuteAsync()
    {
        return await _repository.GetAllUsersAsync();
    }
}
