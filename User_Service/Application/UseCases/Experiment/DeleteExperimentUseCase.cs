using SEP4_User_Service.Application.Interfaces;

namespace SEP4_User_Service.Application.UseCases;

// Denne klasse håndterer sletning af et eksperiment baseret på dets ID.
// Den bruger IExperimentRepository til at udføre sletningen.
public class DeleteExperimentUseCase
{
    private readonly IExperimentRepository _repository;

    // Constructoren initialiserer repository-afhængigheden.
    public DeleteExperimentUseCase(IExperimentRepository repository)
    {
        _repository = repository;
    }

    // Denne metode udfører sletningen af et eksperiment asynkront og returnerer en bool for succes.
    public async Task<bool> ExecuteAsync(int experimentId)
    {
        return await _repository.DeleteAsync(experimentId);
    }
}
