using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces
{
    public interface IUserMissionRepository
    {
        Task<UserMission?> GetByIdAsync(Guid id);
        Task<UserMission?> GetByUserAndMissionAsync(Guid userId, Guid missionId);
        Task<List<UserMission>> GetByUserIdAsync(Guid userId);
        Task<PagedList<UserMission>> GetActiveByUserIdAsync(PageParameters pageParameters, Guid userId);

        Task CreateAsync(UserMission userMission);
        Task UpdateAsync(UserMission userMission);
        Task SoftDeleteAsync(Guid userMissionId);

        // Helpful queries
        Task<bool> IsMissionStartedAsync(Guid userId, Guid missionId);
    }
}