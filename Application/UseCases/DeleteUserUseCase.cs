using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

public class DeleteUserUseCase
{
    private readonly IUserRepository _repository;

    public DeleteUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExecuteAsync(Guid id) 
    {
        return await _repository.DeleteUserAsync(id);
    }
}
