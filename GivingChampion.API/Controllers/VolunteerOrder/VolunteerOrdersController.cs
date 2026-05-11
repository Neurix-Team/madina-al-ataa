using GivingChampion.Application.Interfaces.VolunteerOrderService;
using GivingChampion.Application.DTO.VolunteerOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GivingChampion.Common.Pagination;

namespace GivingChampion.API.Controllers.VolunteerOrder
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VolunteerOrdersController : ControllerBase
    {
        private readonly IVolunteerOrderService _volunteerOrderService;

        public VolunteerOrdersController(IVolunteerOrderService volunteerOrderService)
        {
            _volunteerOrderService = volunteerOrderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _volunteerOrderService.GetAllAsync(pageParameters);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending([FromQuery] PageParameters pageParameters)
        {
            var result = await _volunteerOrderService.GetPendingAsync(pageParameters);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("pending/count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingCount()
        {
            var result = await _volunteerOrderService.GetPendingCountAsync();

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VolunteerOrderDto>> GetById(Guid id)
        {
            var volunteerOrder = await _volunteerOrderService.GetByIdAsync(id);
            return Ok(volunteerOrder);
        }

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveOrder(Guid id)
        {
            var result = await _volunteerOrderService.ApproveOrderAsync(id);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectOrder(
            Guid id,
            [FromBody] RejectVolunteerOrderDto dto)
        {
            var result = await _volunteerOrderService.RejectOrderAsync(
                id,
                dto.RejectionReason);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Volunteer")]
        public async Task<ActionResult<VolunteerOrderDto>> Create(
            [FromBody] CreateVolunteerOrderDto dto)
        {
            var createdVolunteerOrder = await _volunteerOrderService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdVolunteerOrder.Id },
                createdVolunteerOrder
            );
        }

        [HttpPut("{id:guid}/progress")]
        [Authorize(Roles = "Volunteer,Admin")]
        public async Task<IActionResult> UpdateProgress(
            Guid id,
            [FromBody] UpdateVolunteerOrderProgressDto dto)
        {
            var result = await _volunteerOrderService.UpdateProgressAsync(
                id,
                dto.Progress);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _volunteerOrderService.DeleteAsync(id);
            return NoContent();
        }
    }
}