using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces.Mission
{
    public interface IUserMissionService
    {
        Task<Result<UserMissionDto>> StartMissionAsync(StartMissionDto dto, Guid userId);
        Task<Result<UserMissionDto>> UpdateProgressAsync(Guid userMissionId, UpdateProgressDto dto, Guid userId);
        Task<Result<PagedList<UserMissionDto>>> GetMyActiveMissionsAsync(PageParameters pageParameters, Guid userId);
        Task<Result<PagedList<UserMissionDto>>> GetMyCompletedMissionsAsync(PageParameters pageParameters, Guid userId);
        Task<Result<UserMissionDto>> GetByIdAsync(Guid userMissionId, Guid userId);
        //Task<Result<PagedList<UserMissionDto>>> GetAllUserMissionsAsync(PageParameters pageParameters, Guid userId);
    }
}