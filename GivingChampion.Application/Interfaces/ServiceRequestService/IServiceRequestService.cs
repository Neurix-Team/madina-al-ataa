using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GivingChampion.Application.Interfaces.ServiceRequestService
{

    public interface IServiceRequestService
    {
        Task<Result<PagedList<ServiceRequestDto>>> GetAllAsync(PageParameters pageParameters);
        Task<ServiceRequestDto?> GetByIdAsync(Guid id);

        //Task<Result<PagedList<ServiceRequestDto>>> GetByStatusAsync(
        //    RequestStatus status,
        //    PageParameters pageParameters);        //Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId);

        // Gets all service requests filtered by specific status.
        Task<Result<PagedList<ServiceRequestDto>>> GetApprovedRequestsAsync(
                   PageParameters pageParameters);
        Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto);

        Task<bool> DeleteAsync(Guid id);
        //Task<List<ServiceR GetByPartnerIdAsync(Guid partnerId);
    }

}
