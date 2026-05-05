using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IActivityRepository
    {
       
        Task AddAsync(Activity history);
        Task<PagedList<Activity>> GetByEntityIdAsync(Guid entityId, PageParameters pageParameters);
        //Task<PagedList<Activity>> GetByRequestIdAsync(Guid requestId, PageParameters pageParameters);
    }
}
