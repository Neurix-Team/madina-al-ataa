using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Persistance.Interfaces
{
   
        public interface IDonationRequestRepository
        {
            Task<DonationRequest> GetByIdAsync(Guid id);  // Get a single DonationRequest by its ID
            Task<IEnumerable<DonationRequest>> GetAllAsync();  // Get all DonationRequests
            Task<IEnumerable<DonationRequest>> FilterAsync(string searchTerm, bool? isVerified, bool? isFulfilled, UrgencyLevel? urgencyLevel, Guid? partnerId, int pageNumber, int pageSize);  // Filter DonationRequests based on given parameters
            Task AddAsync(DonationRequest donationRequest);  // Add a new DonationRequest
            Task UpdateAsync(DonationRequest donationRequest);  // Update an existing DonationRequest
        }
}

