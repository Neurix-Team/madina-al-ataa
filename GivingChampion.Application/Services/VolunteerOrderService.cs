using AutoMapper;
using GivingChampion.API.Repositories;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.Reward;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Mapper;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace GivingChampion.Application.Services
{
    public class VolunteerOrderService : BaseService, IVolunteerOrderService
    {
        #region Fields

        private readonly IVolunteerOrderRepository _volunteerOrderRepository;
        private readonly IGenericRepository<VolunteerOrder> _genericVolunteerOrderRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRewardSystemService _rewardSystemService;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public VolunteerOrderService(
            IVolunteerOrderRepository volunteerOrderRepository,
            IGenericRepository<VolunteerOrder> genericVolunteerOrderRepository,
            IServiceRequestRepository serviceRequestRepository,
            INotificationService notificationService,
            IRewardSystemService rewardSystemService,
            IHttpContextAccessor httpContextAccessor,
            IActivityService activityService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(httpContextAccessor, activityService)
        {
            _volunteerOrderRepository = volunteerOrderRepository;
            _genericVolunteerOrderRepository = genericVolunteerOrderRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _notificationService = notificationService;
            _rewardSystemService = rewardSystemService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        public async Task<Result<PagedList<VolunteerOrderDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var volunteerOrders = await _volunteerOrderRepository.GetAllAsync(pageParameters);

            return Result<PagedList<VolunteerOrderDto>>.Success(
                _mapper.MapPagedList<VolunteerOrder, VolunteerOrderDto>(volunteerOrders));
        }
        public async Task<Result<PagedList<VolunteerOrderDto>>> GetMyOrdersAsync(PageParameters pageParameters)
        {
            var volunteerOrders = await _volunteerOrderRepository.GetByVolunteerIdAsync(UserId, pageParameters);

            return Result<PagedList<VolunteerOrderDto>>.Success(
                _mapper.MapPagedList<VolunteerOrder, VolunteerOrderDto>(volunteerOrders));
        }
        public async Task<Result<PagedList<VolunteerOrderDto>>> GetPendingAsync(PageParameters pageParameters)
        {
            var volunteerOrders = await _volunteerOrderRepository.GetPendingAsync(pageParameters);

            return Result<PagedList<VolunteerOrderDto>>.Success(
                _mapper.MapPagedList<VolunteerOrder, VolunteerOrderDto>(volunteerOrders));
        }

        public async Task<Result<PendingVolunteerOrderCountDto>> GetPendingCountAsync()
        {
            var count = await _genericVolunteerOrderRepository.CountAsync(
                order => order.Status == OrderStatus.Pending);

            return Result<PendingVolunteerOrderCountDto>.Success(
                new PendingVolunteerOrderCountDto { Exists = count > 0, Count = count });
        }
        #region Get Volunteer Order By Id

        public async Task<VolunteerOrderDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            var volunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (volunteerOrder == null || volunteerOrder.IsDeleted)
                throw new NotFoundException($"Volunteer order with ID {id} was not found.");

            return _mapper.Map<VolunteerOrderDto>(volunteerOrder);
        }

        public async Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto)
        {
            var volunteerId = UserId;

            if (dto == null)
                throw new BadRequestException("Volunteer order create data is required.");

            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            if (dto.ServiceRequestId == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(dto.ServiceRequestId)
                ?? throw new NotFoundException($"Service request with ID {dto.ServiceRequestId} was not found.");

            if (serviceRequest.Status != RequestStatus.Approved)
            {
                throw new BadRequestException(
                    $"Volunteer cannot create an order for this request because its status is {serviceRequest.Status}.");
            }

            var requiredLevelNumber = serviceRequest.RequiredLevel?.Number
               ?? throw new BadRequestException("Service request required level is not assigned.");

            var volunteerLevelNumber = await _volunteerOrderRepository.GetVolunteerLevelNumberAsync(volunteerId)
                ?? throw new BadRequestException("Volunteer level is not assigned.");

            if (volunteerLevelNumber < requiredLevelNumber)
            {
                throw new BadRequestException(
                    $"Your level is not enough to apply for this request. Required level is {requiredLevelNumber}.");
            }
            if (await _volunteerOrderRepository.ExistsActiveByUserAndServiceRequestAsync(
                    volunteerId,
                    dto.ServiceRequestId))
            {
                throw new ConflictException("You already have an active order for this service request.");
            }

            if (await _genericVolunteerOrderRepository.CountAsync(o =>
                    o.ServiceRequestId == dto.ServiceRequestId &&
                    !o.IsDeleted &&
                    o.Status != OrderStatus.Rejected) >= serviceRequest.MaxOrders)
            {
                throw new ConflictException("This service request has reached the maximum number of orders.");
            }

            var volunteerOrder = _mapper.Map<VolunteerOrder>(dto);

            volunteerOrder.UserId = volunteerId;
            volunteerOrder.Status = OrderStatus.Pending;
            volunteerOrder.CreatedAt = DateTime.UtcNow;
            volunteerOrder.IsDeleted = false;

            await _volunteerOrderRepository.AddAsync(volunteerOrder);
            await _unitOfWork.SaveChangesAsync();

            await AddActivityAsync(
                volunteerOrder.Id,
                ActivityEntityType.VolunteerOrder,
                ActivityAction.OrderCreated,
                $"Volunteer order created for service request '{serviceRequest.Title}'.");

            var createdOrder = await _volunteerOrderRepository.GetByIdAsync(volunteerOrder.Id)
                ?? throw new InvalidOperationException("Volunteer order was created but could not be retrieved.");

            return _mapper.Map<VolunteerOrderDto>(createdOrder);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var volunteerId = UserId;

            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            var existingVolunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Volunteer order with ID {id} was not found.");

            if (existingVolunteerOrder.IsDeleted)
                throw new BadRequestException("Volunteer order is already deleted.");

            if (existingVolunteerOrder.UserId != volunteerId)
                throw new UnauthorizedAccessException("You are not allowed to delete this order.");

            if (existingVolunteerOrder.Status != OrderStatus.Pending)
            {
                throw new ConflictException(
                    $"Only pending volunteer orders can be cancelled. Current status is {existingVolunteerOrder.Status}."
                );
            }

            existingVolunteerOrder.IsDeleted = true;
            existingVolunteerOrder.DeletedAt = DateTime.UtcNow;
            existingVolunteerOrder.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(existingVolunteerOrder);
            await _unitOfWork.SaveChangesAsync();

            await AddActivityAsync(
                existingVolunteerOrder.Id,
                ActivityEntityType.VolunteerOrder,
                ActivityAction.OrderCancelled,
                $"Volunteer order cancelled by volunteer '{volunteerId}'.");

            return true;
        }
        public async Task<VolunteerOrderDto?> UpdateProgressAsync(
      Guid orderId,
      int progress)
        {
            var volunteerId = UserId;

            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (progress is <= 0 or > 100)
                throw new BadRequestException("Progress value must be between 1 and 100.");

            var order = await _volunteerOrderRepository.GetByIdForUpdateAsync(orderId)
                ?? throw new NotFoundException($"Volunteer order with ID {orderId} was not found.");

            if (order.IsDeleted)
                throw new BadRequestException("Cannot update progress for a deleted volunteer order.");

            if (order.UserId != volunteerId)
                throw new UnauthorizedAccessException("You are not allowed to update progress for this order.");

            if (order.Status is not (OrderStatus.Approved or OrderStatus.InProgress))
            {
                throw new BadRequestException(
                    $"Only approved or in-progress orders can update progress. Current status is {order.Status}.");
            }

            var serviceRequest = await _serviceRequestRepository.GetByIdForUpdateAsync(order.ServiceRequestId)
                ?? throw new NotFoundException($"Service request with ID {order.ServiceRequestId} was not found.");

            if (serviceRequest.Status == RequestStatus.Completed)
                throw new BadRequestException("Cannot update progress for a completed service request.");

            if (progress < serviceRequest.Progress)
                throw new BadRequestException("Progress cannot be decreased.");

            if (progress == serviceRequest.Progress)
                throw new BadRequestException("Progress value is already the current progress.");

            var shouldRewardServiceRequest = false;

            if (order.Status == OrderStatus.Approved)
            {
                order.Status = OrderStatus.InProgress;
                serviceRequest.Status = RequestStatus.InProgress;

                await AddActivityAsync(
                    order.Id,
                    ActivityEntityType.VolunteerOrder,
                    ActivityAction.TaskStarted,
                    $"Service request '{serviceRequest.Title}' started by volunteer '{order.UserId}'.");
            }

            serviceRequest.Progress = progress;

            await AddActivityAsync(
                order.Id,
                ActivityEntityType.VolunteerOrder,
                ActivityAction.ProgressUpdated,
                $"Service request '{serviceRequest.Title}' progress updated to {progress}%.");

            if (progress == 100)
            {
                serviceRequest.Status = RequestStatus.Completed;
                serviceRequest.Progress = 100;
                shouldRewardServiceRequest = true;

                var eligibleOrders = await _volunteerOrderRepository
                    .GetEligibleByServiceRequestIdAsync(order.ServiceRequestId);

                foreach (var eligibleOrder in eligibleOrders)
                {
                    if (eligibleOrder.Status != OrderStatus.Completed)
                    {
                        eligibleOrder.Status = OrderStatus.Completed;
                        eligibleOrder.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await AddActivityAsync(
                    order.Id,
                    ActivityEntityType.VolunteerOrder,
                    ActivityAction.ProgressUpdated,
                    $"Service request '{serviceRequest.Title}' completed by volunteer '{order.UserId}'.");
            }

            order.UpdatedAt = DateTime.UtcNow;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            _volunteerOrderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            if (shouldRewardServiceRequest)
            {
                await _rewardSystemService.RewardServiceRequestCompletedAsync(serviceRequest.Id);
            }
            // NADA RAFAT
            order.ServiceRequest = serviceRequest;
            // NADA RAFAT
            return _mapper.Map<VolunteerOrderDto>(order);
        }
        #endregion

        #region Approve Volunteer Order

        public async Task<VolunteerOrderDto?> ApproveOrderAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            var order = await _volunteerOrderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException($"Volunteer order with ID {orderId} was not found.");

            if (order.IsDeleted)
                throw new BadRequestException("Cannot approve a deleted volunteer order.");

            if (order.Status != OrderStatus.Pending)
            {
                throw new BadRequestException(
                    $"Only pending orders can be approved. Current status is {order.Status}.");
            }

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(order.ServiceRequestId)
                ?? throw new NotFoundException($"Service request with ID {order.ServiceRequestId} was not found.");

            if (serviceRequest.IsDeleted)
                throw new BadRequestException("Cannot approve an order for a deleted service request.");

            if (serviceRequest.Status is not (RequestStatus.Approved or RequestStatus.Assigned))
            {
                throw new ConflictException(
                    $"Cannot approve this volunteer order because the service request status is {serviceRequest.Status}.");
            }

            var approvedOrdersCount = await _genericVolunteerOrderRepository.CountAsync(o =>
                o.ServiceRequestId == order.ServiceRequestId &&
                !o.IsDeleted &&
                (
                    o.Status == OrderStatus.Approved ||
                    o.Status == OrderStatus.InProgress ||
                    o.Status == OrderStatus.Completed
                ));

            if (approvedOrdersCount >= serviceRequest.MaxOrders)
            {
                throw new ConflictException(
                    "This service request has reached the maximum number of approved volunteer orders.");
            }

            serviceRequest.Status = RequestStatus.Assigned;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            order.Status = OrderStatus.Approved;
            order.ApprovedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await AddActivityAsync(
                order.Id,
                ActivityEntityType.VolunteerOrder,
                ActivityAction.OrderApproved,
                $"Volunteer order for service request '{serviceRequest.Title}' approved for volunteer '{order.UserId}'.");

            await AddActivityAsync(
                order.Id,
                ActivityEntityType.VolunteerOrder,
                ActivityAction.TaskAssigned,
                $"Service request '{serviceRequest.Title}' assigned to volunteer '{order.UserId}'.");

            _volunteerOrderRepository.Update(order);
            await _serviceRequestRepository.UpdateAsync(serviceRequest);

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = order.UserId,
                Title = "Volunteer order approved",
                Message = "Your Volunteer order has been approved.",
                Type = NotificationType.Information,
                LinkedEntityId = order.Id,
                LinkedEntityType = nameof(VolunteerOrder)
            });

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VolunteerOrderDto>(order);
        }
        public async Task<VolunteerOrderDto?> RejectOrderAsync(Guid id, string rejectionReason)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new BadRequestException("Rejection reason is required.");

            rejectionReason = rejectionReason.Trim();

            var order = await _volunteerOrderRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Volunteer order with ID {id} was not found.");

            if (order.IsDeleted)
                throw new BadRequestException("Cannot reject a deleted volunteer order.");

            if (order.Status != OrderStatus.Pending)
                throw new BadRequestException($"Only pending orders can be rejected. Current status is {order.Status}.");

            order.Status = OrderStatus.Rejected;
            order.RejectionReason = rejectionReason;
            order.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(order);

            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = order.UserId,
                Title = "Volunteer order rejected",
                Message = $"Your volunteer order has been rejected. Reason: {rejectionReason}",
                Type = NotificationType.Warning,
                LinkedEntityId = order.Id,
                LinkedEntityType = nameof(VolunteerOrder)
            });

            await AddActivityAsync(
                order.Id,
                ActivityEntityType.VolunteerOrder,
                ActivityAction.OrderRejected,
                $"Volunteer order rejected. Reason: {rejectionReason}");

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VolunteerOrderDto>(order);
        }
        #endregion
    }
}
