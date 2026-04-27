using AutoMapper;
using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Common.DTO.DonationOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Application.Services.DonationOrderService
{
    public class DonationOrderService : IDonationOrderService
    {
        private readonly IDonationOrderRepository _donationOrderRepository;
        private readonly IDonationRequestRepository _donationRequestRepository;
        private readonly IMapper _mapper;

        public DonationOrderService(
            IDonationOrderRepository donationOrderRepository,
            IDonationRequestRepository donationRequestRepository,
            IMapper mapper)
        {
            _donationOrderRepository = donationOrderRepository;
            _donationRequestRepository = donationRequestRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DonationOrderReadDto>> GetAllAsync()
        {
            var donationOrders = await _donationOrderRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<DonationOrderReadDto>>(donationOrders);
        }

        public async Task<IEnumerable<DonationOrderReadDto>> GetMyOrdersAsync(Guid donorUserId)
        {
            var donationOrders = await _donationOrderRepository.GetByDonorIdAsync(donorUserId);

            return _mapper.Map<IEnumerable<DonationOrderReadDto>>(donationOrders);
        }

        public async Task<DonationOrderDetailsDto?> GetByIdAsync(
            Guid id,
            Guid currentUserId,
            bool isAdmin)
        {
            var donationOrder = await _donationOrderRepository.GetByIdAsync(id);

            if (donationOrder == null)
                return null;

            // Admin can view any order.
            // Donor can only view his own order.
            if (!isAdmin && donationOrder.DonorId != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to view this donation order.");

            return _mapper.Map<DonationOrderDetailsDto>(donationOrder);
        }

        public async Task<DonationOrderDetailsDto> CreateAsync(
            CreateDonationOrderDto dto,
            Guid donorUserId)
        {
            if (dto.Amount <= 0)
                throw new InvalidOperationException("Donation amount must be greater than zero.");

            var donationRequest = await _donationRequestRepository.GetByIdAsync(dto.DonationRequestId);

            if (donationRequest == null)
                throw new KeyNotFoundException($"Donation request with ID {dto.DonationRequestId} not found.");

            //if (!donationRequest.IsVerified)
            //    throw new InvalidOperationException("You cannot donate to a donation request that is not approved.");

            //if (donationRequest.IsFulfilled)
            //    throw new InvalidOperationException("This donation request is already fulfilled.");

            if (dto.Amount > donationRequest.AmountRemaining)
                throw new InvalidOperationException("Donation amount cannot be greater than the remaining amount.");

            var donationOrder = _mapper.Map<DonationOrder>(dto);

            donationOrder.Id = Guid.NewGuid();
            donationOrder.DonorId = donorUserId;
            donationOrder.Status = OrderStatus.Pending;
            donationOrder.IsDeleted = false;

            await _donationOrderRepository.CreateAsync(donationOrder);

            var createdOrder = await _donationOrderRepository.GetByIdAsync(donationOrder.Id);

            return _mapper.Map<DonationOrderDetailsDto>(createdOrder);
        }

        public async Task ConfirmAsync(Guid id, Guid donorUserId)
        {
            var donationOrder = await _donationOrderRepository.GetByIdForUpdateAsync(id);

            if (donationOrder == null)
                throw new KeyNotFoundException($"Donation order with ID {id} not found.");

            if (donationOrder.DonorId != donorUserId)
                throw new UnauthorizedAccessException("You are not allowed to confirm this donation order.");

            if (donationOrder.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending donation orders can be confirmed.");

            var donationRequest = await _donationRequestRepository.GetByIdAsync(
                donationOrder.DonationRequestId
            );

            if (donationRequest == null)
                throw new KeyNotFoundException("Related donation request was not found.");

            //if (!donationRequest.IsVerified)
            //    throw new InvalidOperationException("Cannot confirm donation for an unapproved request.");

            //if (donationRequest.IsFulfilled)
            //    throw new InvalidOperationException("This donation request is already fulfilled.");

            if (donationOrder.Amount > donationRequest.AmountRemaining)
                throw new InvalidOperationException("Donation amount is greater than the remaining amount.");

            donationOrder.Status = OrderStatus.Approved;

            donationRequest.AmountRemaining -= donationOrder.Amount;

            if (donationRequest.AmountRemaining <= 0)
            {
                donationRequest.AmountRemaining = 0;
            }

            await _donationOrderRepository.UpdateAsync(donationOrder);
            await _donationRequestRepository.UpdateAsync(donationRequest);
        }

        public async Task CancelAsync(Guid id, Guid currentUserId, bool isAdmin)
        {
            var donationOrder = await _donationOrderRepository.GetByIdForUpdateAsync(id);

            if (donationOrder == null)
                throw new KeyNotFoundException($"Donation order with ID {id} not found.");

            if (!isAdmin && donationOrder.DonorId != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to cancel this donation order.");

            if (donationOrder.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending donation orders can be cancelled.");

            donationOrder.Status = OrderStatus.Rejected;

            await _donationOrderRepository.UpdateAsync(donationOrder);
        }

        public async Task<UpdateDonationOrderDTO> UpdateAsync(Guid id, UpdateDonationOrderDTO dto, Guid donorUserId)
        {
            var donationOrder = await _donationOrderRepository.GetByIdForUpdateAsync(id);

            if (donationOrder == null)
                throw new KeyNotFoundException($"Donation order with ID {id} not found.");
            if (donationOrder.DonorId != donorUserId)
                throw new UnauthorizedAccessException("You are not allowed to update this donation order.");

            _mapper.Map(dto, donationOrder);
            await _donationOrderRepository.UpdateAsync(donationOrder);
            return dto;
        }
    }
}