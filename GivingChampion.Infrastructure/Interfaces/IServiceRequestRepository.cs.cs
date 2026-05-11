using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<PagedList<ServiceRequest>> GetAllAsync(PageParameters pageParameters);

        Task<ServiceRequest?> GetByIdAsync(Guid id);

        Task<PagedList<ServiceRequest>> GetByStatusAsync(
            RequestStatus status,
            PageParameters pageParameters);

        Task<PagedList<ServiceRequest>> GetApprovedRequestsAsync(PageParameters pageParameters);

        Task<List<ServiceRequest>> GetByPartnerIdAsync(Guid partnerId);

        Task<ServiceRequest?> GetByIdForUpdateAsync(Guid id);

        Task AddAsync(ServiceRequest serviceRequest);

        Task UpdateAsync(ServiceRequest serviceRequest);

        Task SoftDeleteAsync(ServiceRequest serviceRequest);

        Task UpdateProgressAsync(Guid serviceRequestId, int progress);
    }
}