using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.DTO.GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces
{
    public interface IActivityService
    {
        Task<Result<PagedList<ActivityDto>>> GetEntityHistory(Guid entityId, PageParameters pageParameters);

        //Task<Result<PagedList<VolunteerHistoryDto>>> GetRequestHistory(
        //    Guid requestId,
        //    PageParameters pageParameters);

        Task AddAsync(CreateActivityDto createActivityDto);
    }
}