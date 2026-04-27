using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{

    public interface IDonationOrderRepository
    {
        Task CreateAsync(DonationOrder donationOrder);

        Task<IEnumerable<DonationOrder>> GetAllAsync();

        Task<IEnumerable<DonationOrder>> GetByDonorIdAsync(Guid donorUserId);

        Task<DonationOrder?> GetByIdAsync(Guid id);

        Task<DonationOrder?> GetByIdForUpdateAsync(Guid id);

        Task UpdateAsync(DonationOrder donationOrder);
    }
}



   

