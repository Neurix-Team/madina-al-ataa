using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.DTO.DonationRequest;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using GivingChampion.Application.DTO.ActivityDto;

namespace GivingChampion.Application.Services
{
    public class DonationRequestService : BaseService, IDonationRequestService
    {
        private readonly IDonationRequestRepository _donationRequestRepository;
        private readonly IActivityService _activityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DonationRequestService(
            IDonationRequestRepository donationRequestRepository,
            IActivityService activityService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, activityService)
        {
            _donationRequestRepository = donationRequestRepository;
            _activityService = activityService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<DonationRequestDto>> GetAllAsync(PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestRepository.GetAllAsync(pageParameters);

            var donationRequestDtos = _mapper.MapPagedList<DonationRequest, DonationRequestDto>(donationRequests);

            return donationRequestDtos;
        }

        public async Task<PagedList<DonationRequestDto>> GetApprovedAsync(PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestRepository.GetApprovedAsync(pageParameters);

            return _mapper.MapPagedList<DonationRequest, DonationRequestDto>(donationRequests);
        }

        public async Task<PagedList<DonationRequestDto>> GetMyRequestsAsync(PageParameters pageParameters)
        {
            var donationRequests = await _donationRequestRepository.GetRequestsByUserAsync(UserId, pageParameters);

            return _mapper.MapPagedList<DonationRequest, DonationRequestDto>(donationRequests);
        }

        public async Task<DonationRequestDto?> GetByIdAsync(
            Guid id,
            bool isAdmin)
        {
            var donationRequest = await _donationRequestRepository.GetByIdAsync(id);

            if (donationRequest == null)
                throw new NotFoundException($"Donation request with ID {id} not found.");

            // Admin can view any request.
            if (isAdmin)
                return _mapper.Map<DonationRequestDto>(donationRequest);

            // Donor can view only approved requests.
            if (donationRequest.Status == RequestStatus.Approved)
                return _mapper.Map<DonationRequestDto>(donationRequest);

            throw new ForbiddenException("You are not allowed to view this donation request.");
        }

        public async Task<DonationRequestDto> AddAsync(
            CreateDonationRequestDto dto,
            bool isAdmin)
        {
            var donationRequest = _mapper.Map<DonationRequest>(dto);

            // If Admin creates it, it is approved directly.
            // If Parent creates it, it waits for approval.
            donationRequest.Status = isAdmin ? RequestStatus.Approved : RequestStatus.Pending;

            if (donationRequest.DonateAmount <= 0)
                throw new BadRequestException("Donation amount must be greater than zero.");

            if (donationRequest.DonateAmount <= 0)
                throw new BadRequestException("Donation amount must be greater than zero.");

            // Usually AmountRemaining should start equal to the target donation amount.
            if (donationRequest.AmountRemaining <= 0)
            {
                donationRequest.AmountRemaining = donationRequest.DonateAmount;
            }

            await _donationRequestRepository.AddAsync(donationRequest);

            await AddActivityAsync(
                donationRequest.Id,
                ActivityEntityType.DonationRequest,
                ActivityAction.DonationRequestCreated,
                $"Created a donation request with title '{donationRequest.Title}' and amount {donationRequest.DonateAmount}.");
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DonationRequestDto>(donationRequest);
        }

        public async Task UpdateAsync(
            Guid id,
            UpdateDonationRequestDto dto,
            bool isAdmin)
        {
            var existingRequest = await _donationRequestRepository.GetByIdAsync(id);

            if (existingRequest == null)
                throw new NotFoundException($"Donation request with ID {id} not found.");

            if (existingRequest.Status == RequestStatus.Approved)
                throw new BadRequestException("You cannot update an approved donation request.");

            if (existingRequest.Status == RequestStatus.Completed)
                throw new BadRequestException("You cannot update a completed donation request.");

            _mapper.Map(dto, existingRequest);

            if (existingRequest.DonateAmount <= 0)
                throw new BadRequestException("Donation amount must be greater than zero.");

            if (existingRequest.AmountRemaining < 0)
                throw new BadRequestException("Amount remaining cannot be negative.");

            await _donationRequestRepository.UpdateAsync(existingRequest);

            await AddActivityAsync(
                existingRequest.Id,
                ActivityEntityType.DonationRequest,
                ActivityAction.DonationRequestUpdated,
                $"Updated a donation request with title '{existingRequest.Title}' and amount {existingRequest.DonateAmount}.");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ApproveAsync(Guid id)
        {
            var donationRequest = await _donationRequestRepository.GetByIdAsync(id);

            if (donationRequest == null)
                throw new NotFoundException($"Donation request with ID {id} not found.");

            if (donationRequest.Status == RequestStatus.Approved ||
                donationRequest.Status == RequestStatus.Completed)
            {
                throw new BadRequestException("Donation request is already approved or completed.");
            }

            donationRequest.Status = RequestStatus.Approved;

            await _donationRequestRepository.UpdateAsync(donationRequest);

            await AddActivityAsync(
                donationRequest.Id,
                ActivityEntityType.DonationRequest,
                ActivityAction.DonationRequestApproved,
                $"Approved a donation request with title '{donationRequest.Title}' and amount {donationRequest.DonateAmount}.");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectAsync(Guid id)
        {
            var donationRequest = await _donationRequestRepository.GetByIdAsync(id);

            if (donationRequest == null)
                throw new NotFoundException($"Donation request with ID {id} not found.");

            if (donationRequest.Status == RequestStatus.Completed)
                throw new BadRequestException("Cannot reject a completed donation request.");

            if (donationRequest.Status == RequestStatus.Cancelled)
                throw new BadRequestException("Donation request is already cancelled.");

            if (donationRequest.Status == RequestStatus.Approved)
                throw new BadRequestException("Cannot reject an approved donation request.");

            donationRequest.Status = RequestStatus.Cancelled;

            await _donationRequestRepository.UpdateAsync(donationRequest);

            await AddActivityAsync(
                donationRequest.Id,
                ActivityEntityType.DonationRequest,
                ActivityAction.DonationRequestRejected,
                $"Rejected a donation request with title '{donationRequest.Title}' and amount {donationRequest.DonateAmount}.");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(
            Guid id,
            bool isAdmin)
        {
            var donationRequest = await _donationRequestRepository.GetByIdAsync(id);

            if (donationRequest == null)
                throw new NotFoundException($"Donation request with ID {id} not found.");

            if (donationRequest.Status == RequestStatus.Approved)
                throw new BadRequestException("You cannot delete an approved donation request.");

            if (donationRequest.Status == RequestStatus.Completed)
                throw new BadRequestException("You cannot delete a completed donation request.");

            await _donationRequestRepository.DeleteAsync(donationRequest);

            await AddActivityAsync(
                donationRequest.Id,
                ActivityEntityType.DonationRequest,
                ActivityAction.DonationRequestDeleted,
                $"Deleted a donation request with title '{donationRequest.Title}' and amount {donationRequest.DonateAmount}.");
            await _unitOfWork.SaveChangesAsync();
        }
    }
}