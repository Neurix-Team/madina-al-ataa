using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;

namespace GivingChampion.Application.Interfaces.VolunteerOrderService
{
    public interface IVolunteerOrderService
    {
        Task<List<VolunteerOrderDto>> GetAllAsync();
        Task<VolunteerOrderDto?> GetByIdAsync(Guid id);

        Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto, Guid volunteerId);
        //Task<VolunteerOrderDto?> UpdateAsync(Guid id, UpdateVolunteerOrderDto dto);
        Task ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus);

        Task<VolunteerOrderDto?> UpdateProgressAsync(Guid orderId, Guid volunteerId, int progress);
        Task<VolunteerOrderDto?> ApproveOrderAsync(Guid id);
        Task<VolunteerOrderDto?> RejectOrderAsync(Guid id, string rejectionReason);

        Task<bool> DeleteAsync(Guid id, Guid volunteerId);
    }
}

