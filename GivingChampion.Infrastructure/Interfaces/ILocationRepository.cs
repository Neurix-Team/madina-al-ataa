using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(Guid id);
        Task<List<Location>> GetAllAsync();
        Task<List<Location>> GetAvailableForLevelAsync(int userLevel);

        Task CreateAsync(Location location);
        Task UpdateAsync(Location location);
        Task SoftDeleteAsync(Guid locationId);
    }
}