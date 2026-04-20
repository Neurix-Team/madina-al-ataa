using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces
{
    public interface IMissionService
    {
        Task<Result<PagedList<MissionDto>>> GetAllActiveAsync();
        Task<Result<List<MissionDto>>> GetAvailableForUserAsync(int userLevel);
        Task<Result<MissionDto>> GetByIdAsync(Guid id);
        Task<Result<MissionDto>> CreateMissionAsync(CreateMissionDto dto);
        Task<Result> UpdateMissionAsync(Guid id, UpdateMissionDto dto);
        Task<Result> SoftDeleteMissionAsync(Guid id);
    }
}