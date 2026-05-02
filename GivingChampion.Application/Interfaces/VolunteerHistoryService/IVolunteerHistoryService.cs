using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces.VolunteerHistoryService
{
    public interface IVolunteerHistoryService
    {
        Task<Result<PagedList<VolunteerHistoryDto>>> GetUserHistory(
            Guid userId,
            PageParameters pageParameters);

        Task<Result<PagedList<VolunteerHistoryDto>>> GetRequestHistory(
            Guid requestId,
            PageParameters pageParameters);

        Task AddAsync(
            Guid userId,
            Guid requestId,
            Guid orderId,
            VolunteerHistoryAction action,
            int? progress = null);
    }
}