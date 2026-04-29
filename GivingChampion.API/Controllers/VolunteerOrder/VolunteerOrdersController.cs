using GivingChampion.API.Extensions;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Persistance.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                _logger.LogError(ex, "Error while retrieving volunteer orders.");

                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving volunteer orders.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Get By Id

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
                _logger.LogError(ex, "Error while retrieving volunteer order {Id}", id);

                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the volunteer order.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #endregion

        #region Approve Volunteer Order

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveOrder(Guid id)
        {
            try
            {
                var result = await _volunteerOrderService.ApproveOrderAsync(id);

                if (result == null)
                    return NotFound(new { message = "Volunteer order not found." });

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

        #endregion

        #region Reject Volunteer Order

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectOrder(Guid id, [FromBody] RejectVolunteerOrderDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _volunteerOrderService.RejectOrderAsync(id, dto.RejectionReason);

                if (result == null)
                    return NotFound(new { message = "Volunteer order not found." });

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
        
        #endregion

        #region Command Endpoints

        #region Create

        [HttpPost]
        [Authorize(Roles = "Volunteer")]
        public async Task<ActionResult<VolunteerOrderDto>> Create([FromBody] CreateVolunteerOrderDto dto)
        {
            try
            {
                if (!User.TryGetCurrentUserId(out var volunteerId))
                    return Unauthorized(new { message = "Invalid or missing volunteer ID in token." });

                var approvedRequests = await _serviceRequestRepository.GetApprovedRequestsAsync();

                var serviceRequest = approvedRequests.FirstOrDefault(sr => sr.Id == dto.ServiceRequestId);

                if (serviceRequest == null)
                    return BadRequest(new { message = "The service request is not approved or not available." });

                var createdVolunteerOrder = await _volunteerOrderService.CreateAsync(dto, volunteerId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdVolunteerOrder.Id },
                    createdVolunteerOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating volunteer order.");

                return StatusCode(500, new
                {
                    message = "An error occurred while creating the volunteer order.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Update Progress

        [Authorize(Roles = "Volunteer, Admin")]
        [HttpPut("{id}/progress")]
        public async Task<IActionResult> UpdateProgress(Guid id, [FromBody] UpdateVolunteerOrderProgressDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var volunteerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(volunteerIdClaim))
                    return Unauthorized(new { message = "User id not found in token." });

                var volunteerId = Guid.Parse(volunteerIdClaim);

                var result = await _volunteerOrderService.UpdateProgressAsync(
                    id,
                    volunteerId,
                    dto.Progress
                );

                if (result == null)
                    return NotFound(new { message = "Volunteer order not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        #endregion

        #region Delete

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (!User.TryGetCurrentUserId(out var volunteerId))
                    return Unauthorized(new { message = "Invalid or missing volunteer ID in token." });

                var deleted = await _volunteerOrderService.DeleteAsync(id, volunteerId);

                if (!deleted)
                    return NotFound(new { message = "Volunteer order not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting volunteer order {Id}", id);

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