using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.Application.Interfaces;

// Denne klasse håndterer oprettelsen af et nyt eksperiment.
// Den bruger IExperimentRepository til at gemme eksperimentet i databasen.
public class CreateExperimentUseCase
{
    private readonly IExperimentRepository _repository;

    // Constructoren initialiserer repository-afhængigheden.
    public CreateExperimentUseCase(IExperimentRepository repository)
    {
        _repository = repository;
    }

    // Denne metode udfører oprettelsen af et eksperiment asynkront.
    public async Task ExecuteAsync(Experiment experiment)
    {
        await _repository.CreateAsync(experiment);
    }
}
