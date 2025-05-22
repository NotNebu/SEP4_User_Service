using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at hente alle brugere fra databasen
public class GetAllUsersUseCase
{
    private readonly IUserRepository _repository;

    // Initialiserer use case med et brugerrepository
    public GetAllUsersUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    // Henter alle brugere fra databasen
    public async Task<List<User>> ExecuteAsync()
    {
        return await _repository.GetAllUsersAsync();
    }
}
