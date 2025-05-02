using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

public class GetUserUseCase
{
   private readonly IUserRepository _repository;

    public GetUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User?> ExecuteByIdAsync(Guid id)
    {
        return await _repository.GetUserByIdAsync(id);
    }

    public async Task<List<User>> ExecuteByIdsAsync(List<Guid> ids)
    {
        return await _repository.GetUsersByIdsAsync(ids);
    }
}