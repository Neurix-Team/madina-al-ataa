using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IAiAvatarRepository
    {
        Task<List<AiAvatar>> GetAllAsync();
        Task<AiAvatar?> GetByIdAsync(Guid id);
        Task AddAsync(AiAvatar aiAvatar);
        void Update(AiAvatar aiAvatar);
        Task SaveChangesAsync();
    }
}