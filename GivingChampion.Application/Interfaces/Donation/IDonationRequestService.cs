using GivingChampion.Application.DTO.DonationRequest;
using GivingChampion.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces
{
     public interface IDonationRequestService
    {
        Task<PagedList<DonationRequestDto>> GetAllAsync(PageParameters paginationParams);

        Task<PagedList<DonationRequestDto>> GetApprovedAsync(PageParameters paginationParams);

        Task<PagedList<DonationRequestDto>> GetMyRequestsAsync(PageParameters paginationParams);
        Task<DonationRequestDto> GetByIdAsync(Guid id, bool isAdmin);
        Task<DonationRequestDto> AddAsync(CreateDonationRequestDto dto, bool isAdmin = false);
        Task UpdateAsync(Guid id, UpdateDonationRequestDto dto, bool isAdmin = false);

        Task ApproveAsync(Guid id);

        Task RejectAsync(Guid id);

        Task SoftDeleteAsync(Guid id, bool isAdmin);
    }

}


