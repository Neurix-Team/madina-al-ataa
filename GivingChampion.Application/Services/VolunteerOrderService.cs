using AutoMapper;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.Enums;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace GivingChampion.Application.Services
{
    public class VolunteerOrderService : BaseService, IVolunteerOrderService
    {
        #region Fields

        private readonly IVolunteerOrderRepository _volunteerOrderRepository;
        private readonly IGenericRepository<VolunteerOrder> _genericVolunteerOrderRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IActivityService _activityService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        #endregion


        #region Constructor

        public VolunteerOrderService(
         IVolunteerOrderRepository volunteerOrderRepository,
         IGenericRepository<VolunteerOrder> genericVolunteerOrderRepository,
         IServiceRequestRepository serviceRequestRepository,
         INotificationService notificationService,

         IHttpContextAccessor httpContextAccessor,
         IActivityService activityService,
         IUnitOfWork unitOfWork,
         IMapper mapper) : base(httpContextAccessor)
        {
            _volunteerOrderRepository = volunteerOrderRepository;
            _genericVolunteerOrderRepository = genericVolunteerOrderRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _notificationRepository = notificationRepository;
            _activityService = activityService;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<VolunteerOrderDto>>> GetAllAsync(PageParameters pageParameters)
        {
            var volunteerOrders = await _volunteerOrderRepository.GetAllAsync(pageParameters);

            var mappedItems = _mapper.Map<IReadOnlyList<VolunteerOrderDto>>(volunteerOrders.Items);

            var pagedDtos = new PagedList<VolunteerOrderDto>(
                mappedItems,
                volunteerOrders.PageNumber,
                volunteerOrders.PageSize,
                volunteerOrders.TotalCount
            );

            return Result<PagedList<VolunteerOrderDto>>.Success(pagedDtos);
        }

        public async Task<Result<PagedList<VolunteerOrderDto>>> GetPendingAsync(PageParameters pageParameters)
        {
            var volunteerOrders = await _volunteerOrderRepository.GetPendingAsync(pageParameters);

            var mappedItems = _mapper.Map<IReadOnlyList<VolunteerOrderDto>>(volunteerOrders.Items);

            var pagedDtos = new PagedList<VolunteerOrderDto>(
                mappedItems,
                volunteerOrders.PageNumber,
                volunteerOrders.PageSize,
                volunteerOrders.TotalCount
            );

            return Result<PagedList<VolunteerOrderDto>>.Success(pagedDtos);
        }

        public async Task<Result<PendingVolunteerOrderCountDto>> GetPendingCountAsync()
        {
            var count = await _genericVolunteerOrderRepository.CountAsync(
                order => order.Status == OrderStatus.Pending
            );

            var dto = new PendingVolunteerOrderCountDto
            {
                Exists = count > 0,
                Count = count
            };

            return Result<PendingVolunteerOrderCountDto>.Success(dto);
        }

        #endregion

        #region Get Volunteer Order By Id

        public async Task<VolunteerOrderDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            var volunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (volunteerOrder == null)
                throw new NotFoundException($"Volunteer order with ID {id} was not found.");

            if (volunteerOrder.IsDeleted)
                throw new NotFoundException($"Volunteer order with ID {id} was not found.");

            return _mapper.Map<VolunteerOrderDto>(volunteerOrder);
        }

        public async Task<VolunteerOrderDto> CreateAsync(
       CreateVolunteerOrderDto dto)
        {
            var volunteerId = UserId;

            if (dto == null)
                throw new BadRequestException("Volunteer order create data is required.");

            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            if (dto.ServiceRequestId == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(dto.ServiceRequestId);

            if (serviceRequest == null)
                throw new NotFoundException($"Service request with ID {dto.ServiceRequestId} was not found.");

            if (serviceRequest.Status != RequestStatus.Approved)
                throw new BadRequestException(
                    $"Volunteer cannot create an order for this request because its status is {serviceRequest.Status}.");

            var alreadyHasActiveOrder = await _volunteerOrderRepository
                .ExistsActiveByUserAndServiceRequestAsync(volunteerId, dto.ServiceRequestId);

            if (alreadyHasActiveOrder)
                throw new ConflictException("You already have an active order for this service request.");

            var volunteerOrder = _mapper.Map<VolunteerOrder>(dto);

            volunteerOrder.UserId = volunteerId;
            volunteerOrder.Status = OrderStatus.Pending;
            volunteerOrder.CreatedAt = DateTime.UtcNow;
            volunteerOrder.IsDeleted = false;

            await _volunteerOrderRepository.AddAsync(volunteerOrder);

            await _unitOfWork.SaveChangesAsync();

            await _activityService.AddAsync(new CreateActivityDto
            {
                UserId = UserId,
                EntityId = volunteerOrder.Id,
                EntityType = ActivityEntityType.VolunteerOrder,
                Action = ActivityAction.OrderCreated,
                Description = $"Volunteer order created for service request '{serviceRequest.Title}'."
            });

            var createdOrder = await _volunteerOrderRepository.GetByIdAsync(volunteerOrder.Id);

            if (createdOrder == null)
                throw new InvalidOperationException("Volunteer order was created but could not be retrieved.");

            return _mapper.Map<VolunteerOrderDto>(createdOrder);
        }
        public async Task ChangeOrderStatusAsync(
            Guid orderId,
            OrderStatus newStatus)
        {
            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (!Enum.IsDefined(typeof(OrderStatus), newStatus))
                throw new BadRequestException("Order status is invalid.");

            var order = await _volunteerOrderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException($"Volunteer order with ID {orderId} was not found.");

            if (order.IsDeleted)
                throw new BadRequestException("Cannot change status for a deleted volunteer order.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();
           



        }

        public async Task<bool> DeleteAsync(
            Guid id)
        {
            var volunteerId = UserId;

            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            var existingVolunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (existingVolunteerOrder == null)
                throw new NotFoundException($"Volunteer order with ID {id} was not found.");

            if (existingVolunteerOrder.IsDeleted)
                throw new BadRequestException("Volunteer order is already deleted.");

            if (existingVolunteerOrder.UserId != volunteerId)
                throw new UnauthorizedAccessException("You are not allowed to delete this order.");

            existingVolunteerOrder.IsDeleted = true;
            existingVolunteerOrder.DeletedAt = DateTime.UtcNow;
            existingVolunteerOrder.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(existingVolunteerOrder);

            await _unitOfWork.SaveChangesAsync();
            await _activityService.AddAsync(new CreateActivityDto
            {
                UserId = volunteerId,
                EntityId = existingVolunteerOrder.Id,
                EntityType = ActivityEntityType.VolunteerOrder,
                Action = ActivityAction.OrderCancelled,
                Description = $"Volunteer order cancelled by volunteer '{volunteerId}'."
            });
            return true;
        }

        public async Task<VolunteerOrderDto?> UpdateProgressAsync(
            Guid orderId,
            int progress)
        {
            var volunteerId = UserId;

            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            if (progress <= 0 || progress > 100)
                throw new BadRequestException("Progress value must be between 1 and 100.");

            var order = await _volunteerOrderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException($"Volunteer order with ID {orderId} was not found.");

            if (order.IsDeleted)
                throw new BadRequestException("Cannot update progress for a deleted volunteer order.");

            if (order.UserId != volunteerId)
                throw new UnauthorizedAccessException("You are not allowed to update progress for this order.");

            if (order.Status != OrderStatus.Approved && order.Status != OrderStatus.InProgress)
                throw new BadRequestException(
                    $"Only approved or in-progress orders can update progress. Current status is {order.Status}.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(order.ServiceRequestId);

            if (serviceRequest == null)
                throw new NotFoundException($"Service request with ID {order.ServiceRequestId} was not found.");

            if (serviceRequest.Status == RequestStatus.Completed)
                throw new BadRequestException("Cannot update progress for a completed service request.");

            if (progress < serviceRequest.Progress)
                throw new BadRequestException("Progress cannot be decreased.");
            var activity = new CreateActivityDto();
            if (order.Status == OrderStatus.Approved)
            {
                order.Status = OrderStatus.InProgress;
                serviceRequest.Status = RequestStatus.InProgress;

                activity = new CreateActivityDto()
                {
                    UserId = order.UserId,
                    EntityId = order.Id,
                    EntityType = ActivityEntityType.VolunteerOrder,
                    Action = ActivityAction.TaskStarted,
                    Description = $"Service request '{serviceRequest.Title}' started by volunteer '{order.UserId}'"
                };

                await _activityService.AddAsync(activity);
            }

            serviceRequest.Progress = progress;

            activity = new CreateActivityDto()
            {
                UserId = order.UserId,
                EntityId = order.Id,
                EntityType = ActivityEntityType.VolunteerOrder,
                Action = ActivityAction.ProgressUpdated,
                Description = $"Service request '{serviceRequest.Title}' progress updated to {progress}%"
            };

            await _activityService.AddAsync(activity);

            if (progress == 100)
            {
                order.Status = OrderStatus.Completed;
                serviceRequest.Status = RequestStatus.Completed;

                activity = new CreateActivityDto()
                {
                    UserId = order.UserId,
                    EntityId = order.Id,
                    EntityType = ActivityEntityType.VolunteerOrder,
                    Action = ActivityAction.ProgressUpdated,
                    Description = $"Service request '{serviceRequest.Title}' progress updated to {progress}%"
                };

                await _activityService.AddAsync(activity);
            }

            order.UpdatedAt = DateTime.UtcNow;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            await _serviceRequestRepository.UpdateAsync(serviceRequest);

            _volunteerOrderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VolunteerOrderDto>(order);
        }

        #endregion

        #region Approve Volunteer Order

        public async Task<VolunteerOrderDto?> ApproveOrderAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            var order = await _volunteerOrderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException($"Volunteer order with ID {orderId} was not found.");

            if (order.IsDeleted)
                throw new BadRequestException("Cannot approve a deleted volunteer order.");

            if (order.Status != OrderStatus.Pending)
                throw new BadRequestException(
                    $"Only pending orders can be approved. Current status is {order.Status}.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(order.ServiceRequestId);

            if (serviceRequest == null)
                throw new NotFoundException($"Service request with ID {order.ServiceRequestId} was not found.");

            serviceRequest.VolunteerUserId = order.UserId;
            serviceRequest.Status = RequestStatus.Assigned;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            order.Status = OrderStatus.Approved;
            order.ApprovedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            var activity = new CreateActivityDto()
            {
                UserId = order.UserId,
                EntityId = order.Id,
                EntityType = ActivityEntityType.VolunteerOrder,
                Action = ActivityAction.OrderApproved,
                Description = $"Volunteer order for service request '{serviceRequest.Title}' approved for volunteer '{order.UserId}'."
            };
            await _activityService.AddAsync(activity);

            activity = new CreateActivityDto()
            {
                UserId = order.UserId,
                EntityId = order.Id,
                EntityType = ActivityEntityType.VolunteerOrder,
                Action = ActivityAction.TaskAssigned,
                Description = $"Service request '{serviceRequest.Title}' assigned to volunteer '{order.UserId}'."
            };
            await _activityService.AddAsync(activity);

            _volunteerOrderRepository.Update(order);

            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto()
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

        public async Task<VolunteerOrderDto?> RejectOrderAsync(
            Guid id,
            string rejectionReason)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new BadRequestException("Rejection reason is required.");

            var order = await _volunteerOrderRepository.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException($"Volunteer order with ID {id} was not found.");

            if (order.IsDeleted)
                throw new BadRequestException("Cannot reject a deleted volunteer order.");

            if (order.Status == OrderStatus.Approved || order.Status == OrderStatus.Completed)
                throw new BadRequestException("Approved or completed orders cannot be rejected.");

            order.Status = OrderStatus.Rejected;
            order.RejectionReason = rejectionReason;
            order.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(order);
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto()
            {
                UserId = order.UserId,
                Title = "Volunteer order rejected",
                Message = $"Your Volunteer order has been rejected. Reason: {rejectionReason}",
                Type = NotificationType.Warning,
                LinkedEntityId = order.Id,
                LinkedEntityType = nameof(VolunteerOrder)
            });

            await _unitOfWork.SaveChangesAsync();

            var activity = new CreateActivityDto()
            {
                UserId = order.UserId,
                EntityId = order.Id,
                EntityType = ActivityEntityType.VolunteerOrder,
                Action = ActivityAction.OrderRejected,
                Description = $"Volunteer order for service request '{order.ServiceRequestId}' rejected. Reason: {rejectionReason}"
            };

            await _activityService.AddAsync(activity);

            return _mapper.Map<VolunteerOrderDto>(order);
        }

        //private async Task CreateOrderStatusNotificationAsync(
        //    VolunteerOrder order,
        //    string title,
        //    string message,
        //    NotificationType type)
        //{
        //    await _notificationRepository.CreateAsync(new Notification
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = order.UserId,
        //        Type = type,
        //        Title = title,
        //        Message = message,
        //        LinkedEntityId = order.Id,
        //        LinkedEntityType = nameof(VolunteerOrder),
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow,
        //        IsDeleted = false
        //    });
        //}

        #endregion
    }
}
