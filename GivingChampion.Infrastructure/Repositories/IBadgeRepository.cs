using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IBadgeRepository
    {
        Task<List<Badge>> GetAllAsync();
        Task<Badge?> GetByIdAsync(Guid id);
        Task AddAsync(Badge badge);
        void Update(Badge badge);
        Task SaveChangesAsync();
    }
}