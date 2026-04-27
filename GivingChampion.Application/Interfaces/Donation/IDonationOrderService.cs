using GivingChampion.Common.DTO.DonationOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.DonationOrderService
{
    public interface IDonationOrderService
    {

        Task<IEnumerable<DonationOrderReadDto>> GetAllAsync();

        Task<IEnumerable<DonationOrderReadDto>> GetMyOrdersAsync(Guid donorUserId);

        Task<DonationOrderDetailsDto?> GetByIdAsync(Guid id, Guid currentUserId, bool isAdmin = false);

        Task<DonationOrderDetailsDto> CreateAsync(CreateDonationOrderDto dto, Guid donorUserId);
        Task<UpdateDonationOrderDTO> UpdateAsync(Guid id, UpdateDonationOrderDTO dto, Guid donorUserId);
        Task ConfirmAsync(Guid id, Guid donorUserId);

        Task CancelAsync(Guid id, Guid currentUserId, bool isAdmin);
    }
}

