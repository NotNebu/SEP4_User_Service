using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

// Denne klasse implementerer repositoryet for eksperimenter.
// Den håndterer CRUD-operationer på eksperimentdata i databasen.
public class ExperimentRepository : IExperimentRepository
{
    private readonly UserDbContext _context;

    // Constructor til at injicere databasekonteksten.
    public ExperimentRepository(UserDbContext context)
    {
        _context = context;
    }

    // Opretter et nyt eksperiment i databasen.
    public async Task CreateAsync(Experiment experiment)
    {
        await _context.Experiments.AddAsync(experiment);
        await _context.SaveChangesAsync();
    }

    // Opdaterer et eksisterende eksperiment i databasen.
    public async Task UpdateAsync(Experiment experiment)
    {
        _context.Experiments.Update(experiment);
        await _context.SaveChangesAsync();
    }

    // Sletter et eksperiment fra databasen baseret på dets ID.
    // Returnerer en bool, der angiver, om sletningen lykkedes.
    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Experiments.FindAsync(id);
        if (entity == null)
            return false;

        _context.Experiments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // Henter et eksperiment fra databasen baseret på dets ID.
    // Returnerer eksperimentet, hvis det findes, ellers null.
    public async Task<Experiment?> GetByIdAsync(int id)
    {
        return await _context.Experiments.FindAsync(id);
    }

    // Henter alle eksperimenter for en given bruger baseret på brugerens ID.
    // Returnerer en liste af eksperimenter.
    public async Task<List<Experiment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Experiments
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }
}
