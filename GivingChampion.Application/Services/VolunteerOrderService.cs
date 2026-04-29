using AutoMapper;
using GivingChampion.Application.Exceptions;
using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Application.Services
{
    public class VolunteerOrderService : IVolunteerOrderService
    {
        #region Fields

        private readonly IVolunteerOrderRepository _volunteerOrderRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IVolunteerHistoryService _historyService;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public VolunteerOrderService(
            IVolunteerOrderRepository volunteerOrderRepository,
            IServiceRequestRepository serviceRequestRepository,
            IVolunteerHistoryService historyService,
            IMapper mapper)
        {
            _volunteerOrderRepository = volunteerOrderRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _historyService = historyService;
            _mapper = mapper;
        }

        #endregion

        #region Get All Volunteer Orders

        public async Task<List<VolunteerOrderDto>> GetAllAsync()
        {
            var volunteerOrders = await _volunteerOrderRepository.GetAllAsync();

            return _mapper.Map<List<VolunteerOrderDto>>(volunteerOrders);
        }

        #endregion

        #region Get Volunteer Order By Id

        public async Task<VolunteerOrderDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            var volunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (volunteerOrder == null)
                throw new NotFoundException("Volunteer order not found.");

            return _mapper.Map<VolunteerOrderDto>(volunteerOrder);
        }

        #endregion

        #region Create Volunteer Order

        public async Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto, Guid volunteerId)
        {
            if (dto == null)
                throw new BadRequestException("Volunteer order data is required.");

            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            if (dto.ServiceRequestId == Guid.Empty)
                throw new BadRequestException("Service request ID is required.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(dto.ServiceRequestId);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            if (serviceRequest.Status != RequestStatus.Approved)
            {
                throw new ConflictException(
                    $"Volunteer cannot create an order for this request because its current status is {serviceRequest.Status}."
                );
            }

            var alreadyHasActiveOrder = await _volunteerOrderRepository
                .ExistsActiveByUserAndServiceRequestAsync(volunteerId, dto.ServiceRequestId);

            if (alreadyHasActiveOrder)
                throw new ConflictException("You already have an active order for this service request.");

            var volunteerOrder = _mapper.Map<VolunteerOrder>(dto);

            volunteerOrder.UserId = volunteerId;
            volunteerOrder.Status = OrderStatus.Pending;
            volunteerOrder.CreatedAt = DateTime.UtcNow;

            await _volunteerOrderRepository.AddAsync(volunteerOrder);
            await _volunteerOrderRepository.SaveChangesAsync();

            var createdOrder = await _volunteerOrderRepository.GetByIdAsync(volunteerOrder.Id);

            if (createdOrder == null)
                throw new InvalidOperationException("Volunteer order was created but could not be retrieved.");

            return _mapper.Map<VolunteerOrderDto>(createdOrder);
        }

        #endregion

        #region Change Volunteer Order Status

        public async Task ChangeOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            var order = await _volunteerOrderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Volunteer order not found.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(order);
            await _volunteerOrderRepository.SaveChangesAsync();
        }

        #endregion

        #region Delete Volunteer Order

        public async Task<bool> DeleteAsync(Guid id, Guid volunteerId)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            var existingVolunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (existingVolunteerOrder == null)
                throw new NotFoundException("Volunteer order not found.");

            if (existingVolunteerOrder.UserId != volunteerId)
                throw new UnauthorizedAccessException("You are not allowed to delete this order.");

            existingVolunteerOrder.IsDeleted = true;
            existingVolunteerOrder.DeletedAt = DateTime.UtcNow;
            existingVolunteerOrder.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(existingVolunteerOrder);
            await _volunteerOrderRepository.SaveChangesAsync();

            return true;
        }

        #endregion

        #region Update Volunteer Order Progress

        public async Task<VolunteerOrderDto?> UpdateProgressAsync(Guid orderId, Guid volunteerId, int progress)
        {
            if (orderId == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (volunteerId == Guid.Empty)
                throw new BadRequestException("Volunteer ID is required.");

            var order = await _volunteerOrderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Volunteer order not found.");

            if (order.UserId != volunteerId)
                throw new UnauthorizedAccessException("You are not allowed to update progress for this order.");

            if (progress <= 0)
                throw new BadRequestException("Progress value must be a positive integer.");

            if (progress > 100)
                throw new BadRequestException("Progress value cannot exceed 100.");

            if (order.Status != OrderStatus.Approved && order.Status != OrderStatus.InProgress)
            {
                throw new ConflictException(
                    $"Only approved or in-progress orders can update progress. Current status is {order.Status}."
                );
            }

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(order.ServiceRequestId);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            if (serviceRequest.Status == RequestStatus.Completed)
                throw new ConflictException("Cannot update progress for a completed service request.");

            if (progress < serviceRequest.Progress)
                throw new BadRequestException("Progress cannot be decreased.");

            if (order.Status == OrderStatus.Approved)
            {
                order.Status = OrderStatus.InProgress;
                serviceRequest.Status = RequestStatus.InProgress;

                await _historyService.AddAsync(
                    order.UserId,
                    serviceRequest.Id,
                    order.Id,
                    VolunteerHistoryAction.TaskStarted
                );
            }

            serviceRequest.Progress = progress;

            await _historyService.AddAsync(
                order.UserId,
                serviceRequest.Id,
                order.Id,
                VolunteerHistoryAction.ProgressUpdated,
                progress
            );

            if (progress == 100)
            {
                order.Status = OrderStatus.Completed;
                serviceRequest.Status = RequestStatus.Completed;

                await _historyService.AddAsync(
                    order.UserId,
                    serviceRequest.Id,
                    order.Id,
                    VolunteerHistoryAction.TaskCompleted
                );
            }

            order.UpdatedAt = DateTime.UtcNow;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            _volunteerOrderRepository.Update(order);

            await _serviceRequestRepository.SaveChangesAsync();
            await _volunteerOrderRepository.SaveChangesAsync();

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
                throw new NotFoundException("Volunteer order not found.");

            if (order.Status != OrderStatus.Pending)
            {
                throw new ConflictException(
                    $"Only pending orders can be approved. Current status is {order.Status}."
                );
            }

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(order.ServiceRequestId);

            if (serviceRequest == null)
                throw new NotFoundException("Service request not found.");

            serviceRequest.VolunteerUserId = order.UserId;
            serviceRequest.Status = RequestStatus.Assigned;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            order.Status = OrderStatus.Approved;
            order.ApprovedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            await _historyService.AddAsync(
                order.UserId,
                serviceRequest.Id,
                order.Id,
                VolunteerHistoryAction.OrderApproved
            );

            await _historyService.AddAsync(
                order.UserId,
                serviceRequest.Id,
                order.Id,
                VolunteerHistoryAction.TaskAssigned
            );

            _volunteerOrderRepository.Update(order);
            await _serviceRequestRepository.UpdateAsync(serviceRequest);

            await _serviceRequestRepository.SaveChangesAsync();
            await _volunteerOrderRepository.SaveChangesAsync();

            return _mapper.Map<VolunteerOrderDto>(order);
        }

        #endregion

        #region Reject Volunteer Order

        public async Task<VolunteerOrderDto?> RejectOrderAsync(Guid id, string rejectionReason)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("Volunteer order ID is required.");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new BadRequestException("Rejection reason is required.");

            var order = await _volunteerOrderRepository.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException("Volunteer order not found.");

            if (order.Status == OrderStatus.Approved || order.Status == OrderStatus.Completed)
            {
                throw new ConflictException(
                    $"Cannot reject this order because its current status is {order.Status}."
                );
            }

            order.Status = OrderStatus.Rejected;
            order.RejectionReason = rejectionReason;
            order.UpdatedAt = DateTime.UtcNow;

            _volunteerOrderRepository.Update(order);
            await _volunteerOrderRepository.SaveChangesAsync();

            await _historyService.AddAsync(
                order.UserId,
                order.ServiceRequestId,
                order.Id,
                VolunteerHistoryAction.OrderRejected
            );

            return _mapper.Map<VolunteerOrderDto>(order);
        }

        #endregion
    }
}