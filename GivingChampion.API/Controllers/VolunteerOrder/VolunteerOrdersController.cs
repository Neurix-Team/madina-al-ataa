using GivingChampion.API.Extensions;
using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Application.DTO.VolunteerOrder;
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

        #endregion

        #region Constructor

        public VolunteerOrdersController(IVolunteerOrderService volunteerOrderService)
        {
            _volunteerOrderService = volunteerOrderService;
        }

        #endregion

        #region Get All Volunteer Orders

        [HttpGet]
        public async Task<ActionResult<List<VolunteerOrderDto>>> GetAll()
        {
            var volunteerOrders = await _volunteerOrderService.GetAllAsync();

            return Ok(volunteerOrders);
        }

        #endregion

        #region Get Volunteer Order By Id

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VolunteerOrderDto>> GetById(Guid id)
        {
            var volunteerOrder = await _volunteerOrderService.GetByIdAsync(id);

            return Ok(volunteerOrder);
        }

        #endregion

        #region Approve Volunteer Order

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveOrder(Guid id)
        {
            var result = await _volunteerOrderService.ApproveOrderAsync(id);

            return Ok(result);
        }

        #endregion

        #region Reject Volunteer Order

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectOrder(Guid id, [FromBody] RejectVolunteerOrderDto dto)
        {
            var result = await _volunteerOrderService.RejectOrderAsync(
                id,
                dto.RejectionReason
            );

            return Ok(result);
        }

        #endregion

        #region Create Volunteer Order

        [HttpPost]
        [Authorize(Roles = "Volunteer")]
        public async Task<ActionResult<VolunteerOrderDto>> Create([FromBody] CreateVolunteerOrderDto dto)
        {
            if (!User.TryGetCurrentUserId(out var volunteerId))
                return Unauthorized(new { message = "Invalid or missing volunteer ID in token." });

            var createdVolunteerOrder = await _volunteerOrderService.CreateAsync(
                dto,
                volunteerId
            );

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdVolunteerOrder.Id },
                createdVolunteerOrder
            );
        }

        #endregion

        #region Update Volunteer Order Progress

        [HttpPut("{id:guid}/progress")]
        [Authorize(Roles = "Volunteer, Admin")]
        public async Task<IActionResult> UpdateProgress(
            Guid id,
            [FromBody] UpdateVolunteerOrderProgressDto dto)
        {
            if (!User.TryGetCurrentUserId(out var volunteerId))
                return Unauthorized(new { message = "Invalid or missing user ID in token." });

            var result = await _volunteerOrderService.UpdateProgressAsync(
                id,
                volunteerId,
                dto.Progress
            );

            return Ok(result);
        }

        #endregion

        #region Delete Volunteer Order

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!User.TryGetCurrentUserId(out var volunteerId))
                return Unauthorized(new { message = "Invalid or missing volunteer ID in token." });

            await _volunteerOrderService.DeleteAsync(id, volunteerId);

            return NoContent();
        }

        #endregion
    }
}