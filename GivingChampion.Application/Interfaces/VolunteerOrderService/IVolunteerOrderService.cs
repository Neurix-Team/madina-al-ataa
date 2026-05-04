using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;

namespace GivingChampion.Application.Interfaces.VolunteerOrderService
{
    public interface IVolunteerOrderService
    {
        Task<Result<PagedList<VolunteerOrderDto>>> GetAllAsync(PageParameters pageParameters);
        Task<Result<PagedList<VolunteerOrderDto>>> GetPendingAsync(PageParameters pageParameters);
        Task<Result<PendingVolunteerOrderCountDto>> GetPendingCountAsync();
        Task<VolunteerOrderDto?> GetByIdAsync(Guid id);

        Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto);
        //Task<VolunteerOrderDto?> UpdateAsync(Guid id, UpdateVolunteerOrderDto dto);
        Task ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus);

        Task<VolunteerOrderDto?> UpdateProgressAsync(Guid orderId, int progress);
        Task<VolunteerOrderDto?> ApproveOrderAsync(Guid id);
        Task<VolunteerOrderDto?> RejectOrderAsync(Guid id, string rejectionReason);

        Task<bool> DeleteAsync(Guid id);
    }
}

