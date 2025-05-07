using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at oprette en ny bruger.
public class CreateUserUseCase
{
    private readonly IUserRepository _repository;

    // Initialiserer use case med et brugerrepository.
    public CreateUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    // Opretter en ny bruger og returnerer det oprettede brugerobjekt.
    public async Task<User> ExecuteAsync(User user)
    {
        await _repository.CreateUserAsync(user);
        return user;
    }
}
