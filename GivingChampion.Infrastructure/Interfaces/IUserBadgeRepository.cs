using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IUserBadgeRepository
    {
        Task<List<UserBadge>> GetAllByProfileIdAsync(Guid profileId);

        Task<UserBadge?> GetByIdAsync(Guid id);

        Task<UserBadge?> GetByProfileAndBadgeAsync( Guid profileId,Guid badgeId,bool includeDeleted = false);

        Task AddAsync(UserBadge userBadge);

        void Update(UserBadge userBadge);

        Task DeleteAsync(UserBadge userBadge);

        Task SaveChangesAsync();
    }
}