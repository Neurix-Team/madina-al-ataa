using System;
using System.Collections.Generic;
using GivingChampion.Common.DTO.ServiceRequestDto;
using System.Threading.Tasks;
using GivingChampion.Common.Enums;
namespace GivingChampion.Application.Interfaces.ServiceRequestService
{

    public interface IServiceRequestService
    {
        Task<List<ServiceRequestDto>> GetAllAsync();

        Task<ServiceRequestDto?> GetByIdAsync(Guid id);

        //Task<List<ServiceRequestDto>> GetPendingAsync();
        public Task<List<ServiceRequestDto>> GetByStatusAsync(RequestStatus status);

        //Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId);

        // Gets all service requests filtered by specific status.
        public Task<List<ServiceRequestDto>> GetApprovedRequestsAsync();
        Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto);

        Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto);

        Task<bool> DeleteAsync(Guid id);
        //Task<List<ServiceR GetByPartnerIdAsync(Guid partnerId);
    }

}
