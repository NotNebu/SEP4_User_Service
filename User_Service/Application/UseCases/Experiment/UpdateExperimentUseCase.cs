using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

// Denne klasse håndterer opdatering af et eksisterende eksperiment.
// Den bruger IExperimentRepository til at opdatere eksperimentet i databasen.
public class UpdateExperimentUseCase
{
    private readonly IExperimentRepository _repository;

    // Constructoren initialiserer repository-afhængigheden.
    public UpdateExperimentUseCase(IExperimentRepository repository)
    {
        _repository = repository;
    }

    // Denne metode udfører opdateringen af et eksperiment asynkront.
    public async Task ExecuteAsync(Experiment experiment)
    {
        await _repository.UpdateAsync(experiment);
    }
}
