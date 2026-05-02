using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IAiAvatarRepository
    {
        Task<List<AiAvatar>> GetAllAsync();
        Task<AiAvatar?> GetByIdAsync(Guid id);
        Task AddAsync(AiAvatar aiAvatar);
        void Update(AiAvatar aiAvatar);
    }
}
