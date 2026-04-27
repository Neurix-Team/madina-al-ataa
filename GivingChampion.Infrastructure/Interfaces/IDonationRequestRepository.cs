using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{
   
    public interface IDonationRequestRepository
    {
        Task AddAsync(DonationRequest donationRequest);

        Task<PagedList<DonationRequest>> GetAllAsync(PageParameters pageParameters);

        Task<PagedList<DonationRequest>> GetApprovedAsync(PageParameters pageParameters);
        Task<PagedList<DonationRequest>> GetRequestsByUserAsync(Guid userId, PageParameters pageParameters);

        Task<DonationRequest?> GetByIdAsync(Guid id);

        //Task<DonationRequest?> GetByIdForUpdateAsync(Guid id);

        Task UpdateAsync(DonationRequest donationRequest);
        Task DeleteAsync(DonationRequest donationRequest);
    }
}

