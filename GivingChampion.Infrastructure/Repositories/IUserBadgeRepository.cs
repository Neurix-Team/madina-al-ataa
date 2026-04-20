using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IUserBadgeRepository
    {
        Task<List<UserBadge>> GetAllByProfileIdAsync(Guid profileId);
        Task<UserBadge?> GetByIdAsync(Guid id);
        Task AddAsync(UserBadge userBadge);
        void Update(UserBadge userBadge);
        Task SaveChangesAsync();
    }
}