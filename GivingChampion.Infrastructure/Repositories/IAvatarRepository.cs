using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IAvatarRepository
    {
        Task<List<Avatar>> GetAllAsync();
        Task<Avatar?> GetByIdAsync(Guid id);
        Task AddAsync(Avatar avatar);
        void Update(Avatar avatar);
        Task SaveChangesAsync();
    }
}