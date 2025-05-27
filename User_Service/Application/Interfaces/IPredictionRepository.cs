using SEP4_User_Service.Domain.Entities;

namespace SEP4_User_Service.Application.Interfaces
{
    public interface IPredictionRepository
    {
        Task<List<Prediction>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Prediction prediction);
        Task DeleteAsync(Prediction prediction);
        Task<Prediction?> GetByIdAsync(int id); // til delete
    }
}
