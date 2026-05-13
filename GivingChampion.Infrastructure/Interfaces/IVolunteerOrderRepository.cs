using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerOrderRepository
    {
        Task<PagedList<VolunteerOrder>> GetAllAsync(PageParameters pageParameters);
        Task<PagedList<VolunteerOrder>> GetPendingAsync(PageParameters pageParameters);
        Task<VolunteerOrder?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<PagedList<VolunteerOrder>> GetByVolunteerIdAsync(
     Guid volunteerId,
     PageParameters pageParameters);
        Task<int?> GetVolunteerLevelNumberAsync(Guid volunteerUserId);
        Task AddAsync(VolunteerOrder volunteerOrder);
        void Update(VolunteerOrder volunteerOrder);
        Task<bool> ExistsActiveByUserAndServiceRequestAsync(Guid userId, Guid serviceRequestId);
    }
}
