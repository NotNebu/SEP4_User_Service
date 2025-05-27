using Microsoft.EntityFrameworkCore;
using SEP4_User_Service.Application.Interfaces;
using SEP4_User_Service.Domain.Entities;
using SEP4_User_Service.Infrastructure.Persistence.Data;

namespace SEP4_User_Service.Infrastructure.Persistence.Repositories
{
    public class PredictionRepository : IPredictionRepository
    {
        private readonly UserDbContext _context;

        public PredictionRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<List<Prediction>> GetByUserIdAsync(Guid userId)
        {
            return await _context
                .Predictions.Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Timestamp)
                .ToListAsync();
        }

        public async Task AddAsync(Prediction prediction)
        {
            await _context.Predictions.AddAsync(prediction);
            await _context.SaveChangesAsync();
        }

        public async Task<Prediction?> GetByIdAsync(int id)
        {
            return await _context.Predictions.FindAsync(id);
        }

        public async Task DeleteAsync(Prediction prediction)
        {
            _context.Predictions.Remove(prediction);
            await _context.SaveChangesAsync();
        }
    }
}
