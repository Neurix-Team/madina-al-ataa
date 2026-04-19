using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface ILevelRepository
    {
        Task<List<Level>> GetAllAsync();
        Task<Level?> GetByIdAsync(Guid id);
        Task AddAsync(Level level);
        void Update(Level level);
        Task SaveChangesAsync();
    }
}