using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.DonationOrderService;
using GivingChampion.Application.DTO.DonationOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Application.DTO.ActivityDto;

namespace GivingChampion.Application.Services.DonationOrderService
{
    public class DonationOrderService : BaseService, IDonationOrderService
    {
        private readonly IDonationOrderRepository _donationOrderRepository;
        private readonly IDonationRequestRepository _donationRequestRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly INotificationService _notificationService;
        private readonly IActivityService _activityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DonationOrderService(
            IDonationOrderRepository donationOrderRepository,
            IDonationRequestRepository donationRequestRepository,
            IDonorRepository donorRepository,
            IActivityService activityService,
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _donationOrderRepository = donationOrderRepository;
            _donationRequestRepository = donationRequestRepository;
            _donorRepository = donorRepository;
            _activityService = activityService;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<DonationOrderReadDto>>> GetAllAsync(
            PageParameters pageParameters)
        {
            var donationOrders = await _donationOrderRepository.GetAllAsync(pageParameters);

            var result = _mapper.MapPagedList<DonationOrder, DonationOrderReadDto>(donationOrders);

            return Result<PagedList<DonationOrderReadDto>>.Success(result);
        }

        public async Task<Result<PagedList<DonationOrderReadDto>>> GetMyOrdersAsync(
            PageParameters pageParameters)
        {
            var donationOrders = await _donationOrderRepository.GetByDonorIdAsync(
                UserId,
                pageParameters);

            var result = _mapper.MapPagedList<DonationOrder, DonationOrderReadDto>(donationOrders);

            return Result<PagedList<DonationOrderReadDto>>.Success(result);
        }

        public async Task<Result<DonationOrderDetailsDto?>> GetByIdAsync(
            Guid id,
            bool isAdmin = false)
        {
            var donationOrder = await _donationOrderRepository.GetByIdAsync(id);

            if (donationOrder == null)
                throw new NotFoundException($"Donation order with ID {id} was not found.");

            if (!isAdmin && donationOrder.DonorId != UserId)
                throw new UnauthorizedAccessException("You are not allowed to view this donation order.");

            var dto = _mapper.Map<DonationOrderDetailsDto>(donationOrder);

            return Result<DonationOrderDetailsDto?>.Success(dto);
        }

        public async Task<Result<DonationOrderDetailsDto>> CreateAsync(
            CreateDonationOrderDto dto)
        {
            if (dto == null)
                throw new BadRequestException("Donation order data is required.");

            if (dto.Amount <= 0)
                throw new BadRequestException("Donation amount must be greater than zero.");

            var donationRequest = await _donationRequestRepository.GetByIdAsync(dto.DonationRequestId);

            if (donationRequest == null)
                throw new NotFoundException($"Donation request with ID {dto.DonationRequestId} was not found.");

            if (donationRequest.Status != RequestStatus.Approved)
                throw new BadRequestException("You can donate only to approved donation requests.");

            if (donationRequest.Status == RequestStatus.Completed)
                throw new BadRequestException("This donation request is already completed.");

            if (dto.Amount > donationRequest.AmountRemaining)
                throw new BadRequestException("Donation amount cannot be greater than the remaining amount.");

            var donationOrder = _mapper.Map<DonationOrder>(dto);

            donationOrder.DonorId = UserId;
            donationOrder.Status = OrderStatus.Pending;

            await _donationOrderRepository.CreateAsync(donationOrder);

            await AddActivityAsync(donationOrder.Id, ActivityEntityType.DonationOrder, ActivityAction.DonationOrderCreated, $"Created a donation order of {donationOrder.Amount} {donationOrder.Currency} for request '{donationRequest.Title}'.");

            await _unitOfWork.SaveChangesAsync();

            var createdOrder = await _donationOrderRepository.GetByIdAsync(donationOrder.Id)
                ?? throw new NotFoundException("Created donation order could not be retrieved.");

            var createdDto = _mapper.Map<DonationOrderDetailsDto>(createdOrder);

            return Result<DonationOrderDetailsDto>.Success(createdDto);
        }

        public async Task<Result<UpdateDonationOrderDTO>> UpdateAsync(
            Guid id,
            UpdateDonationOrderDTO dto)
        {
            if (dto == null)
                throw new BadRequestException("Donation order data is required.");

            var donationOrder = await _donationOrderRepository.GetByIdAsync(id);

            if (donationOrder == null)
                throw new NotFoundException($"Donation order with ID {id} was not found.");

            if (donationOrder.DonorId != UserId)
                throw new UnauthorizedAccessException("You are not allowed to update this donation order.");

            if (donationOrder.Status != OrderStatus.Pending)
                throw new BadRequestException("Only pending donation orders can be updated.");

            if (dto.Amount <= 0)
                throw new BadRequestException("Donation amount must be greater than zero.");

            var donationRequest = await _donationRequestRepository.GetByIdAsync(
                donationOrder.DonationRequestId);

            if (donationRequest == null)
                throw new NotFoundException("Related donation request was not found.");

            if (donationRequest.Status != RequestStatus.Approved)
                throw new BadRequestException("Cannot update donation order for a request that is not approved.");

            if (dto.Amount > donationRequest.AmountRemaining)
                throw new BadRequestException("Donation amount cannot be greater than the remaining amount.");

            _mapper.Map(dto, donationOrder);

            await _donationOrderRepository.UpdateAsync(donationOrder);

            await AddActivityAsync(donationOrder.Id, ActivityEntityType.DonationOrder, ActivityAction.DonationOrderUpdated, $"Updated a donation order of {donationOrder.Amount} {donationOrder.Currency} for request '{donationRequest.Title}'.");

            await _unitOfWork.SaveChangesAsync();

            var updatedDto = _mapper.Map<UpdateDonationOrderDTO>(donationOrder);

            return Result<UpdateDonationOrderDTO>.Success(updatedDto);
        }

        public async Task<Result<DonationOrderDetailsDto>> ApproveAsync(Guid id)
        {
            var donationOrder = await _donationOrderRepository.GetByIdAsync(id);

            if (donationOrder == null)
                throw new NotFoundException($"Donation order with ID {id} was not found.");

            if (donationOrder.Status != OrderStatus.Pending)
                throw new BadRequestException("Only pending donation orders can be approved.");

            var donationRequest = await _donationRequestRepository.GetByIdAsync(
                donationOrder.DonationRequestId);

            if (donationRequest == null)
                throw new NotFoundException("Related donation request was not found.");

            if (donationRequest.Status != RequestStatus.Approved)
                throw new BadRequestException("Cannot approve donation for a request that is not approved.");

            if (donationRequest.Status == RequestStatus.Completed)
                throw new BadRequestException("This donation request is already completed.");

            if (donationOrder.Amount > donationRequest.AmountRemaining)
                throw new BadRequestException("Donation amount is greater than the remaining amount.");

            donationOrder.Status = OrderStatus.Approved;

            var donor = await _donorRepository.GetByUserIdAsync(donationOrder.DonorId);

            if (donor == null)
                throw new NotFoundException("Donor was not found.");

            donor.TotalDonated += donationOrder.Amount;

            donationRequest.AmountRemaining -= donationOrder.Amount;

            if (donationRequest.AmountRemaining <= 0)
            {
                donationRequest.AmountRemaining = 0;
                donationRequest.Status = RequestStatus.Completed;
            }

            await _donorRepository.UpdateAsync(donor);
            await _donationOrderRepository.UpdateAsync(donationOrder);
            await _donationRequestRepository.UpdateAsync(donationRequest);

            await AddActivityAsync(donationOrder.Id, ActivityEntityType.DonationOrder, ActivityAction.DonationOrderApproved, $"Approved a donation order of {donationOrder.Amount} {donationOrder.Currency} for request '{donationRequest.Title}'.");

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto()
            {
                UserId = donationOrder.DonorId,
                Title = "Donation order approved",
                Message = "Your donation order has been approved.",
                Type = NotificationType.Information,
                LinkedEntityId = donationOrder.Id,
                LinkedEntityType = nameof(DonationOrder)
            });

            await _unitOfWork.SaveChangesAsync();

            var updatedOrder = await _donationOrderRepository.GetByIdAsync(id);

            if (updatedOrder == null)
                throw new NotFoundException("Updated donation order could not be retrieved.");

            var dto = _mapper.Map<DonationOrderDetailsDto>(updatedOrder);

            return Result<DonationOrderDetailsDto>.Success(dto);
        }

        public async Task<Result<DonationOrderDetailsDto>> RejectAsync(Guid id)
        {
            var donationOrder = await _donationOrderRepository.GetByIdAsync(id);

            if (donationOrder == null)
                throw new NotFoundException($"Donation order with ID {id} was not found.");

            if (donationOrder.Status != OrderStatus.Pending)
                throw new BadRequestException("Only pending donation orders can be rejected.");

            donationOrder.Status = OrderStatus.Rejected;

            await _donationOrderRepository.UpdateAsync(donationOrder);
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto()
            {
                UserId = donationOrder.DonorId,
                Title = "Donation order Rejected",
                Message = "Your donation order has been Rejected.",
                Type = NotificationType.Information,
                LinkedEntityId = donationOrder.Id,
                LinkedEntityType = nameof(DonationOrder)
            });

            await AddActivityAsync(donationOrder.Id, ActivityEntityType.DonationOrder, ActivityAction.DonationOrderRejected, $"Rejected a donation order of {donationOrder.Amount} {donationOrder.Currency} for request '{donationOrder.DonationRequest.Title}'.");

            await _unitOfWork.SaveChangesAsync();

            var updatedOrder = await _donationOrderRepository.GetByIdAsync(id);

            if (updatedOrder == null)
                throw new NotFoundException("Updated donation order could not be retrieved.");

            var dto = _mapper.Map<DonationOrderDetailsDto>(updatedOrder);

            return Result<DonationOrderDetailsDto>.Success(dto);
        }
    }
}