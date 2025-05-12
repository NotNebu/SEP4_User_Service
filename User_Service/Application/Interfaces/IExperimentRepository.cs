using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces;

// Dette interface definerer kontrakten for repositoryet, der håndterer eksperimentdata.
// Det bruges til at udføre CRUD-operationer på eksperimenter i databasen.
public interface IExperimentRepository
{
    // Opretter et nyt eksperiment i databasen.
    Task CreateAsync(Experiment experiment);

    // Opdaterer et eksisterende eksperiment i databasen.
    Task UpdateAsync(Experiment experiment);

    // Sletter et eksperiment fra databasen baseret på dets ID.
    // Returnerer en bool, der angiver, om sletningen lykkedes.
    Task<bool> DeleteAsync(int id);

    // Henter et eksperiment fra databasen baseret på dets ID.
    // Returnerer eksperimentet, hvis det findes, ellers null.
    Task<Experiment?> GetByIdAsync(int id);

    // Henter alle eksperimenter for en given bruger baseret på brugerens ID.
    // Returnerer en liste af eksperimenter.
    Task<List<Experiment>> GetByUserIdAsync(Guid userId);
}
