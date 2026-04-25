using AutoMapper;
using GivingChampion.Application.Interfaces.VolunteerHistoryService;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using GivingChampion.Persistance.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Services
{
    /// <summary>
    /// Service implementation for managing volunteer orders.
    /// Handles business logic and uses AutoMapper for entity/DTO conversion.
    /// </summary>
    public class VolunteerOrderService : IVolunteerOrderService
    {
        #region Fields

        private readonly IVolunteerOrderRepository _volunteerOrderRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IVolunteerHistoryService _historyService;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VolunteerOrderService"/> class.
        /// </summary>
        /// <param name="volunteerOrderRepository">Volunteer order repository.</param>
        /// <param name="mapper">AutoMapper instance.</param>
        public VolunteerOrderService(
            IVolunteerOrderRepository volunteerOrderRepository,
            IServiceRequestRepository serviceRequestRepository,IVolunteerHistoryService historyService,
            IMapper mapper)
        {
            _volunteerOrderRepository = volunteerOrderRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _historyService = historyService;
            _mapper = mapper;
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Gets all volunteer orders that are not soft deleted.
        /// </summary>
        /// <returns>List of volunteer order DTOs.</returns>
        public async Task<List<VolunteerOrderDto>> GetAllAsync()
        {
            var volunteerOrders = await _volunteerOrderRepository.GetAllAsync();
            return _mapper.Map<List<VolunteerOrderDto>>(volunteerOrders);
        }

        /// <summary>
        /// Gets a single volunteer order by its id.
        /// Returns null if the order does not exist or is soft deleted.
        /// </summary>
        /// <param name="id">Volunteer order id.</param>
        /// <returns>Volunteer order DTO if found; otherwise null.</returns>
        public async Task<VolunteerOrderDto?> GetByIdAsync(Guid id)
        {
            var volunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (volunteerOrder == null)
                return null;

            return _mapper.Map<VolunteerOrderDto>(volunteerOrder);
        }

        #endregion

        #region Command Methods

        #region Create
        /// <summary>
        /// Creates a new volunteer order.
        /// VolunteerId is taken from the authenticated user's JWT token.
        /// </summary>
        /// <param name="dto">Create volunteer order DTO.</param>
        /// <param name="volunteerId">Volunteer id from JWT token.</param>
        /// <returns>The created volunteer order DTO.</returns>
        public async Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto, Guid volunteerId)
        {
            // Ensure the related service request exists and is in an allowed status
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(dto.ServiceRequestId);
            if (serviceRequest == null)
                throw new InvalidOperationException("Service request not found.");

            // Only allow creating an order when the request status is Approved
            if (!(serviceRequest.Status == RequestStatus.Approved ))
                throw new InvalidOperationException("Volunteer cannot create an order for this request because its status is not Approved.");

            var volunteerOrder = _mapper.Map<VolunteerOrder>(dto);


            // Use repository create which saves changes
            await _volunteerOrderRepository.CreateAsync(volunteerOrder);

            return _mapper.Map<VolunteerOrderDto>(volunteerOrder);
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates an existing volunteer order by its id.
        /// Returns null if the order does not exist or is soft deleted.
        /// </summary>
        /// <param name="id">Volunteer order id.</param>
        /// <param name="dto">Update volunteer order DTO.</param>
        /// <returns>The updated volunteer order DTO if found; otherwise null.</returns>
        public async Task<VolunteerOrderDto?> UpdateAsync(Guid id, UpdateVolunteerOrderDto dto)
        {
            var existingVolunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (existingVolunteerOrder == null)
                return null;

            _mapper.Map(dto, existingVolunteerOrder);

            _volunteerOrderRepository.Update(existingVolunteerOrder);
            await _volunteerOrderRepository.SaveChangesAsync();

            return _mapper.Map<VolunteerOrderDto>(existingVolunteerOrder);
        }
        #endregion

        #region Delete

        /// <summary>
        /// Soft deletes a volunteer order by its id.
        /// Returns true if deleted successfully, otherwise false.
        /// </summary>
        /// <param name="id">Volunteer order id.</param>
        /// <returns>True if deleted successfully; otherwise false.</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingVolunteerOrder = await _volunteerOrderRepository.GetByIdAsync(id);

            if (existingVolunteerOrder == null)
                return false;

            _volunteerOrderRepository.SoftDelete(existingVolunteerOrder);
            await _volunteerOrderRepository.SaveChangesAsync();

            return true;
        }
        #endregion

        public async Task<VolunteerOrderDto?> UpdateProgressAsync(Guid orderId, int addedProgress)
        {
            var order = await _volunteerOrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return null;

            if (order.Status == OrderStatus.Rejected)
                throw new Exception("Cannot update a rejected order.");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(order.ServiceRequestId);
            if (serviceRequest == null)
                throw new Exception("Service request not found.");

            var newProgress = serviceRequest.Progress + addedProgress;

            if (newProgress > 100)
                newProgress = 100;

            if (order.Status == OrderStatus.Approved && serviceRequest.Progress == 0)
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

            serviceRequest.Progress = newProgress;

            await _historyService.AddAsync(
                order.UserId,
                serviceRequest.Id,
                order.Id,
                VolunteerHistoryAction.ProgressUpdated,
                newProgress
            );

            if (newProgress == 100)
            {
                serviceRequest.Status = RequestStatus.Completed;

                await _historyService.AddAsync(
                    order.UserId,
                    serviceRequest.Id,
                    order.Id,
                    VolunteerHistoryAction.TaskCompleted
                );
            }

            _serviceRequestRepository.Update(serviceRequest);
            _volunteerOrderRepository.Update(order);

            await _serviceRequestRepository.SaveChangesAsync();
            await _volunteerOrderRepository.SaveChangesAsync();

            return _mapper.Map<VolunteerOrderDto>(order);
        }
        #region ProcessRequest

        public async Task<VolunteerOrderDto?> ApproveOrderAsync(Guid orderId)
        {
            var order = await _volunteerOrderRepository.GetByIdAsync(orderId);
            if (order == null)
                return null;

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Order already processed");

            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(order.ServiceRequestId);
            if (serviceRequest == null)
                throw new Exception("Service request not found");

            serviceRequest.VolunteerUserId = order.UserId;
            serviceRequest.Status = RequestStatus.Assigned;

            await _historyService.AddAsync(
   order.UserId,
   serviceRequest.Id,
   order.Id,
   VolunteerHistoryAction.OrderApproved
);
            // history
            await _historyService.AddAsync(
                order.UserId,
                serviceRequest.Id,
                order.Id,
                VolunteerHistoryAction.TaskAssigned
            );
            order.Status = OrderStatus.Approved;
            order.ApprovedAt = DateTime.UtcNow;
           

            _volunteerOrderRepository.Update(order);
            _serviceRequestRepository.Update(serviceRequest);

            await _serviceRequestRepository.SaveChangesAsync();
            await _volunteerOrderRepository.SaveChangesAsync();

           
            return _mapper.Map<VolunteerOrderDto>(order);
        }

        public async Task<VolunteerOrderDto?> RejectOrderAsync(Guid id, string rejectionReason)
        {
            var order = await _volunteerOrderRepository.GetByIdAsync(id);
            if (order == null)
                return null;

            order.Status = OrderStatus.Rejected;
            order.RejectionReason = rejectionReason;

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
        #endregion
    }
}