using GivingChampion.Common.DTO.DonationRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.DonationRequest
{
     public interface IDonationRequestService
    {
            Task<ReadDonationRequestDto> GetByIdAsync(Guid id);  // Get a DonationRequest by its ID
            Task<IEnumerable<ListDonationRequestDto>> GetAllAsync();  // Get all DonationRequests
            Task<ReadDonationRequestDto> AddAsync(CreateDonationRequestDto donationRequestDTO); // Ensure it returns Task<ReadDonationRequestDto>
        Task UpdateAsync(Guid id, UpdateDonationRequestDto donationRequestDTO);  // Update an existing DonationRequest
    }

}


