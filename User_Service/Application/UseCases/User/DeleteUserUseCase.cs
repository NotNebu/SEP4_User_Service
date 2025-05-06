using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.Application.UseCases;

// Use case til at slette en bruger.
public class DeleteUserUseCase
{
    private readonly IUserRepository _repository;

    // Initialiserer use case med et brugerrepository.
    public DeleteUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    // Sletter en bruger baseret på ID.
    // Returnerer true, hvis sletningen lykkes; ellers false.
    public async Task<bool> ExecuteAsync(Guid id) 
    {
        return await _repository.DeleteUserAsync(id);
    }
}
