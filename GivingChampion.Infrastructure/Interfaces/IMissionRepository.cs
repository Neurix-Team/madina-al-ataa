using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Application.Interfaces
{
    public interface IMissionRepository
    {
        Task<Mission?> GetByIdAsync(Guid id);
        Task<List<Mission>> GetAllActiveAsync();
        Task<List<Mission>> GetByDifficultyAsync(DifficultyLevel difficulty);
        Task<List<Mission>> GetAvailableForLevelAsync(int userLevel);

        Task CreateAsync(Mission mission);
        Task UpdateAsync(Mission mission);
        Task SoftDeleteAsync(Guid missionId);
    }
}