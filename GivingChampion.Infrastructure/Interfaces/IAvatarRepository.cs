using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IAvatarRepository
    {
        Task<List<Avatar>> GetAllAsync();
        Task<Avatar?> GetByIdAsync(Guid id);
        Task<Avatar> AddAsync(Guid profileId);
        void Update(Avatar avatar);
        Task SaveChangesAsync();
    }
}