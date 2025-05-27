using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at hente en bruger baseret på ID eller en liste af IDs
public class GetUserUseCase
{
    private readonly IUserRepository _repository;

    // Initialiserer use case med et brugerrepository
    public GetUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    // Henter en bruger baseret på ID
    public async Task<User?> ExecuteByIdAsync(Guid id)
    {
        return await _repository.GetUserByIdAsync(id);
    }

    // Henter brugere baseret på en liste af IDs
    public async Task<List<User>> ExecuteByIdsAsync(List<Guid> ids)
    {
        return await _repository.GetUsersByIdsAsync(ids);
    }
}
