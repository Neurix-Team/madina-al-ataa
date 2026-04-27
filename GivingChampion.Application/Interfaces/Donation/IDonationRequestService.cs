using GivingChampion.Common.DTO.DonationRequest;
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

        Task<PagedList<DonationRequestDto>> GetMyRequestsAsync(Guid UserId, PageParameters paginationParams);
        Task<DonationRequestDto> GetByIdAsync(Guid id, Guid currentUserId, bool isAdmin);
        Task<DonationRequestDto> AddAsync(CreateDonationRequestDto dto, Guid currentUserId, bool isAdmin = false);
        Task UpdateAsync(Guid id, UpdateDonationRequestDto dto, Guid currentUserId, bool isAdmin = false);

        Task ApproveAsync(Guid id);

        Task RejectAsync(Guid id);

        Task SoftDeleteAsync(Guid id, Guid currentUserId, bool isAdmin);
    }

}


