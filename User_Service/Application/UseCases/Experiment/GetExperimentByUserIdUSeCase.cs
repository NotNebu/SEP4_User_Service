using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.UseCases;

// Denne klasse henter et eksperiment baseret på dets ID.
// Den bruger IExperimentRepository til at hente data fra databasen.
public class GetExperimentByIdUseCase
{
    private readonly IExperimentRepository _repository;

    // Constructoren initialiserer repository-afhængigheden.
    public GetExperimentByIdUseCase(IExperimentRepository repository)
    {
        _repository = repository;
    }

    // Denne metode henter et eksperiment asynkront og returnerer det, hvis det findes.
    public async Task<Experiment?> ExecuteAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
