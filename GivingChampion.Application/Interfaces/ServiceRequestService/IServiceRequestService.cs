using System;
using System.Collections.Generic;
using GivingChampion.Common.DTO.ServiceRequestDto;
using System.Threading.Tasks;
namespace GivingChampion.Application.Interfaces.ServiceRequestService
{

    public interface IServiceRequestService
    {
        Task<List<ServiceRequestDto>> GetAllAsync();

        Task<ServiceRequestDto?> GetByIdAsync(Guid id);

        Task<List<ServiceRequestDto>> GetPendingAsync();

        Task<List<ServiceRequestDto>> GetByPartnerIdAsync(Guid partnerId);

        Task<ServiceRequestDto> CreateAsync(CreateServiceRequestDto dto);

        Task<bool> UpdateAsync(Guid id, UpdateServiceRequestDto dto);

        Task<bool> DeleteAsync(Guid id);
    }

}
