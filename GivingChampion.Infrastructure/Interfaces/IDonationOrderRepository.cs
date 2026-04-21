using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{

        public interface IDonationOrderRepository
        {
            Task<DonationOrder> GetByIdAsync(Guid id);  // For getting a single donation order by id
            Task<IEnumerable<DonationOrder>> GetAllAsync();  // Now matches the return type of IEnumerable
            Task CreateAsync(DonationOrder donationOrder);  // Now matches method name in repository
            Task UpdateAsync(DonationOrder donationOrder);  // For updating a donation order
        }
    }



   

