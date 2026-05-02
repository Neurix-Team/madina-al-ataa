using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using GivingChampion.Application.DTO.ServiceRequestDto;
using System.Threading.Tasks;
namespace GivingChampion.Application.Interfaces.ServiceRequestService
{

    public interface IServiceRequestService
    {
        Task<Result<PagedList<ServiceRequestDto>>> GetAllAsync(PageParameters pageParameters);
        Task<ServiceRequestDto?> GetByIdAsync(Guid id);

        //Task<List<ServiceRequestDto>> GetPendingAsync();
        Task<Result<PagedList<ServiceRequestDto>>> GetByStatusAsync(
            RequestStatus status,
            PageParameters pageParameters);        //Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId);

        // Gets all service requests filtered by specific status.
        public Task<List<ServiceRequestDto>> GetApprovedRequestsAsync();
        Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto);

        Task<bool> DeleteAsync(Guid id);
        //Task<List<ServiceR GetByPartnerIdAsync(Guid partnerId);
    }

}
