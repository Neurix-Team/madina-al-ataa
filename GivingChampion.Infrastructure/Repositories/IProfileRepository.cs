using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IProfileRepository
    {
        Task<List<Profile>> GetAllAsync();
        Task<Profile?> GetByIdAsync(Guid id);
        Task AddAsync(Profile profile);
        void Update(Profile profile);
        Task SaveChangesAsync();
    }
}