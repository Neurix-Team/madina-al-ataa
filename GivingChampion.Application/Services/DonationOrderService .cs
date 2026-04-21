using AutoMapper;
using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Common.DTO.DonationOrder;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services.DonationOrderService
{
    public class DonationOrderService : IDonationOrderService
    {
        private readonly IDonationOrderRepository _donationOrderRepository;
        private readonly IMapper _mapper;

        // Constructor - Inject the repository and AutoMapper
        public DonationOrderService(IDonationOrderRepository donationOrderRepository, IMapper mapper)
        {
            _donationOrderRepository = donationOrderRepository;
            _mapper = mapper;
        }

        #region Query Methods

        // Get DonationOrder by Id
        public async Task<DonationOrderDetailsDto> GetByIdAsync(Guid id)
        {
            var donationOrder = await _donationOrderRepository.GetByIdAsync(id);
            if (donationOrder == null)
            {
                throw new KeyNotFoundException($"DonationOrder with ID {id} not found.");
            }

            return _mapper.Map<DonationOrderDetailsDto>(donationOrder);
        }

        // Get all DonationOrders
        public async Task<IEnumerable<DonationOrderReadDto>> GetAllAsync()
        {
            var donationOrders = await _donationOrderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DonationOrderReadDto>>(donationOrders);
        }

        #endregion

        #region Command Methods

        // Create a new DonationOrder
        public async Task CreateAsync(CreateDonationOrderDto donationOrderDto)
        {
            // Map the DTO to the entity
            var donationOrder = _mapper.Map<DonationOrder>(donationOrderDto);

            // Add the donation order to the repository
            await _donationOrderRepository.CreateAsync(donationOrder);
        }

        // Update an existing DonationOrder
        public async Task UpdateAsync(UpdateDonationOrderDTO donationOrderDto, Guid id)
        {
            var existingDonationOrder = await _donationOrderRepository.GetByIdAsync(id);
            if (existingDonationOrder == null)
            {
                throw new KeyNotFoundException($"DonationOrder with ID {id} not found.");
            }

            // Map the updated DTO to the existing entity
            _mapper.Map(donationOrderDto, existingDonationOrder);

            // Update the donation order in the repository
            await _donationOrderRepository.UpdateAsync(existingDonationOrder);
        }

        #endregion
    }
}