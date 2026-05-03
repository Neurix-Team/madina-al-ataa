using GivingChampion.Application.DTO.Mission;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces.Mission
{
    public interface IUserMissionService
    {
        Task<Result<UserMissionDto>> StartMissionAsync(StartMissionDto dto);
        Task<Result<UserMissionDto>> UpdateProgressAsync(Guid userMissionId, UpdateProgressDto dto);
        Task<Result<PagedList<UserMissionDto>>> GetMyActiveMissionsAsync(PageParameters pageParameters);
        Task<Result<PagedList<UserMissionDto>>> GetMyCompletedMissionsAsync(PageParameters pageParameters);
        Task<Result<UserMissionDto>> GetByIdAsync(Guid userMissionId);
        //Task<Result<PagedList<UserMissionDto>>> GetAllUserMissionsAsync(PageParameters pageParameters, Guid userId);
    }
}
