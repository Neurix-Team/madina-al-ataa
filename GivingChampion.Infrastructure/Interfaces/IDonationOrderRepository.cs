using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{

        public interface IDonationOrderRepository
        {
        Task CreateAsync(DonationOrder donationOrder);
        Task<PagedList<DonationOrder>> GetAllAsync(PageParameters pageParameters);
        Task<PagedList<DonationOrder>> GetByDonorIdAsync(
            Guid donorUserId,
            PageParameters pageParameters);
        Task<DonationOrder?> GetByIdAsync(Guid id);
        Task UpdateAsync(DonationOrder donationOrder);
        }
    }



   

