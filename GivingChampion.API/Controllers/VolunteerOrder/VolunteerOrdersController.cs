using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Common.DTO.VolunteerOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        #endregion

        #region Constructor

        public VolunteerOrdersController(IVolunteerOrderService volunteerOrderService)
        {
            _volunteerOrderService = volunteerOrderService;
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
            var result = await _volunteerOrderService.ApproveOrderAsync(id);
            if (result == null)
            {
                return NotFound("Volunteer order not found.");
            }
            return Ok(result);
        }
        // Reject Volunteer Order
        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectOrder(Guid id, [FromBody] string rejectionReason)
        {
            var result = await _volunteerOrderService.RejectOrderAsync(id, rejectionReason);
            if (result == null)
            {
                return NotFound("Volunteer order not found.");
            }
            return Ok(result);
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

        #region Update

        /// <summary>
        /// Update an existing volunteer order.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Volunteer")]
        public async Task<ActionResult<VolunteerOrderDto>> Update(Guid id, [FromBody] UpdateVolunteerOrderDto dto)
        {
            try
            {
                var updatedVolunteerOrder = await _volunteerOrderService.UpdateAsync(id, dto);

                if (updatedVolunteerOrder == null)
                    return NotFound(new { message = "Volunteer order not found." });

                return Ok(updatedVolunteerOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while updating the volunteer order.",
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