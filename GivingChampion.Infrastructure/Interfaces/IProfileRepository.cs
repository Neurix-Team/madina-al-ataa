using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IProfileRepository
    {
        Task<List<Profile>> GetAllAsync();
        Task<Profile?> GetByIdAsync(Guid id);
        Task<Profile> AddAsync(Guid userId);
        Task Update(Profile profile);
        Task SaveChangesAsync();
    }
}