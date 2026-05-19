using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces
{
    public interface IMissionRepository
    {
        Task<Mission?> GetByIdAsync(Guid id);
        Task<PagedList<Mission>> GetAllOpenAsync(PageParameters pageParameters);
        Task<PagedList<Mission>> GetByDifficultyAsync(DifficultyLevel difficulty, PageParameters pageParameters);
        Task<PagedList<Mission>> GetAvailableForLevelAsync(int userLevel, PageParameters pageParameters);

        Task CreateAsync(Mission mission);
        Task UpdateAsync(Mission mission);
        Task SoftDeleteAsync(Guid missionId);
    }
}