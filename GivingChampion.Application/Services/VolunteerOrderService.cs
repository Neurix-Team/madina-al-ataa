using AutoMapper;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.DTO.VolunteerOrder;
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
            IMapper mapper)
        {
            _volunteerOrderRepository = volunteerOrderRepository;
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
            var volunteerOrder = _mapper.Map<VolunteerOrder>(dto);

            volunteerOrder.VolunteerId = volunteerId;
            volunteerOrder.Status = OrderStatus.Pending;
            volunteerOrder.IsDeleted = false;
            volunteerOrder.DeletedAt = null;
            volunteerOrder.CreatedAt = DateTime.UtcNow;

            await _volunteerOrderRepository.AddAsync(volunteerOrder);
            await _volunteerOrderRepository.SaveChangesAsync();

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

        #endregion
    }
}

