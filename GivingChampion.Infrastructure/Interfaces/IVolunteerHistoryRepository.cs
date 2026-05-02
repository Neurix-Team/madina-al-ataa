using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerHistoryRepository
    {
        Task AddAsync(VolunteerHistories history);

        Task<PagedList<VolunteerHistories>> GetByUserIdAsync(
            Guid userId,
            PageParameters pageParameters);

        Task<PagedList<VolunteerHistories>> GetByRequestIdAsync(
            Guid requestId,
            PageParameters pageParameters);

        Task SaveChangesAsync();
    }
}