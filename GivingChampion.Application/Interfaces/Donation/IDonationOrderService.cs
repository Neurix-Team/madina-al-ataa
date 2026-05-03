using GivingChampion.Application.DTO.DonationOrder;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.DonationOrderService
{
    public interface IDonationOrderService
    {

        Task<Result<PagedList<DonationOrderReadDto>>> GetAllAsync(PageParameters pageParameters);

        Task<Result<PagedList<DonationOrderReadDto>>> GetMyOrdersAsync(PageParameters pageParameters);

        Task<Result<DonationOrderDetailsDto?>> GetByIdAsync(Guid id, bool isAdmin = false);

        Task<Result<DonationOrderDetailsDto>> CreateAsync(CreateDonationOrderDto dto);
        Task<Result<UpdateDonationOrderDTO>> UpdateAsync(Guid id, UpdateDonationOrderDTO dto);
        Task<Result<DonationOrderDetailsDto>> ApproveAsync(Guid id);

        Task<Result<DonationOrderDetailsDto>> RejectAsync(Guid id);
    }
}

