using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpYaml.Serialization.Logging;
using System.Security.Claims;

namespace GivingChampion.API.Controllers.VolunteerOrder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerOrdersController : ControllerBase
    {
        #region Fields

        private readonly IVolunteerOrderService _volunteerOrderService;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ILogger<VolunteerOrdersController> _logger;
        #endregion

        #region Constructor

        public VolunteerOrdersController(
        IVolunteerOrderService volunteerOrderService,
        IServiceRequestRepository serviceRequestRepository,
        ILogger<VolunteerOrdersController> logger)
        {
            _volunteerOrderService = volunteerOrderService;
            _serviceRequestRepository = serviceRequestRepository;
            _logger = logger;
        }

        #endregion

        #region Query Endpoints

        #region Get All

        /// <summary>
        /// Get all volunteer orders.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<VolunteerOrderDto>>> GetAll()
        {
            try
            {
                var volunteerOrders = await _volunteerOrderService.GetAllAsync();
                return Ok(volunteerOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving volunteer orders.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Get By Id

        /// <summary>
        /// Get volunteer order by id.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VolunteerOrderDto>> GetById(Guid id)
        {
            try
            {
                var volunteerOrder = await _volunteerOrderService.GetByIdAsync(id);

                if (volunteerOrder == null)
                    return NotFound(new { message = "Volunteer order not found." });

                return Ok(volunteerOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the volunteer order.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #endregion




        // Approve Volunteer Order
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveOrder(Guid id)
        {
            try
            {
                var result = await _volunteerOrderService.ApproveOrderAsync(id);

                if (result == null)
                    return NotFound("Volunteer order not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while approving order {Id}", id);

                return StatusCode(500, new
                {
                    message = "An error occurred while approving order.",
                    error = ex.Message
                });
            }
        }
        // Reject Volunteer Order
        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectOrder(Guid id, [FromBody] string rejectionReason)
        {
            try
            {
                var result = await _volunteerOrderService.RejectOrderAsync(id, rejectionReason);

                if (result == null)
                    return NotFound("Volunteer order not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while rejecting order {Id}", id);

                return StatusCode(500, new
                {
                    message = "An error occurred while rejecting order.",
                    error = ex.Message
                });
            }
        }
        #region Command Endpoints

        #region Create

        /// <summary>
        /// Create a new volunteer order.
        /// VolunteerId is taken from the authenticated user's token.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Volunteer")]
        public async Task<ActionResult<VolunteerOrderDto>> Create([FromBody] CreateVolunteerOrderDto dto)
        {
            try
            {
                var volunteerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(volunteerIdClaim) || !Guid.TryParse(volunteerIdClaim, out var volunteerId))
                    return Unauthorized(new { message = "Invalid or missing volunteer ID in token." });

                var approvedRequests = await   _serviceRequestRepository.GetApprovedRequestsAsync();

                var serviceRequest = approvedRequests.FirstOrDefault(sr => sr.Id == dto.ServiceRequestId);

                if (serviceRequest == null)
                {
                    return BadRequest(new { message = "The service request is not approved or not available." });
                }

                var createdVolunteerOrder = await _volunteerOrderService.CreateAsync(dto, volunteerId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdVolunteerOrder.Id },
                    createdVolunteerOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the volunteer order.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Update Progress

        /// <summary>
        /// Update volunteer order progress
        /// </summary>
        [HttpPatch("{id}/progress")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> UpdateProgress(Guid id, [FromBody] int addedProgress)
        {
            try
            {
                var result = await _volunteerOrderService.UpdateProgressAsync(id, addedProgress);

                if (result == null)
                    return NotFound("Volunteer order not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while updating progress",
                    error = ex.Message
                });
            }
        }

        #endregion



        #region Delete

        /// <summary>
        /// Soft delete a volunteer order.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _volunteerOrderService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Volunteer order not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the volunteer order.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #endregion
    }
}