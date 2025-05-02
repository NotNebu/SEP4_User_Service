using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at opdatere en brugers data
public class UpdateUserUseCase
{
    private readonly IUserRepository _repository;

    // Initialiserer use case med et brugerrepository
    public UpdateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    // Opdaterer en brugers data i databasen
    public async Task ExecuteAsync(User user)
    {
        await _repository.UpdateUserAsync(user);
    }
}
