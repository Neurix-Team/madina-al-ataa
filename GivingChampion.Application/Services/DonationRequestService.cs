using AutoMapper;
using GivingChampion.Application.Interfaces.DonationRequest;
using GivingChampion.Common.DTO.DonationRequest;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services
{
    public class DonationRequestService : IDonationRequestService
    {
        private readonly IDonationRequestRepository _donationRequestRepository;
        private readonly IMapper _mapper;

        // Constructor to inject dependencies (Repository and AutoMapper)
        public DonationRequestService(IDonationRequestRepository donationRequestRepository, IMapper mapper)
        {
            _donationRequestRepository = donationRequestRepository;
            _mapper = mapper;
        }

        #region Query Methods

        // Get a DonationRequest by its ID
        public async Task<ReadDonationRequestDto> GetByIdAsync(Guid id)
        {
            var donationRequest = await _donationRequestRepository.GetByIdAsync(id);
            if (donationRequest == null)
            {
                throw new KeyNotFoundException($"DonationRequest with ID {id} not found.");
            }

            return _mapper.Map<ReadDonationRequestDto>(donationRequest);
        }

        // Get all DonationRequests
        public async Task<IEnumerable<ListDonationRequestDto>> GetAllAsync()
        {
            var donationRequests = await _donationRequestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListDonationRequestDto>>(donationRequests);
        }

        #endregion

        #region Command Methods

        // Add a new DonationRequest
        public async Task<ReadDonationRequestDto> AddAsync(CreateDonationRequestDto donationRequestDTO)
        {
            var donationRequest = _mapper.Map<DonationRequest>(donationRequestDTO);

            await _donationRequestRepository.AddAsync(donationRequest);  

            return _mapper.Map<ReadDonationRequestDto>(donationRequest);
        }

        public async Task UpdateAsync(Guid id, UpdateDonationRequestDto donationRequestDTO)
        {
            var existingRequest = await _donationRequestRepository.GetByIdAsync(id);
            if (existingRequest == null)
            {
                throw new KeyNotFoundException($"DonationRequest with ID {id} not found.");
            }

            // Map the values from DTO to the existing DonationRequest
            _mapper.Map(donationRequestDTO, existingRequest);

            await _donationRequestRepository.UpdateAsync(existingRequest);
        }

        #endregion
    }
}