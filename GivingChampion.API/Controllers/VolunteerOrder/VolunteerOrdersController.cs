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
    /// <summary>
    /// Handles HTTP requests for VolunteerOrders.
    /// </summary>
    public class VolunteerOrdersController : ControllerBase
    {
        private readonly IVolunteerOrderService _volunteerOrderService;

        /// <summary>
        /// Performs the VolunteerOrdersController operation.
        /// </summary>
        /// <param name="volunteerOrderService">Provides the volunteerOrderService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public VolunteerOrdersController(IVolunteerOrderService volunteerOrderService)
        {
            _volunteerOrderService = volunteerOrderService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var result = await _volunteerOrderService.GetAllAsync(pageParameters);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Performs the GetPending operation.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetPending([FromQuery] PageParameters pageParameters)
        {
            var result = await _volunteerOrderService.GetPendingAsync(pageParameters);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("pending/count")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Performs the GetPendingCount operation.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetPendingCount()
        {
            var result = await _volunteerOrderService.GetPendingCountAsync();

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<VolunteerOrderDto>> GetById(Guid id)
        {
            var volunteerOrder = await _volunteerOrderService.GetByIdAsync(id);
            return Ok(volunteerOrder);
        }

        [HttpPatch("{id:guid}/approve")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Approves the specified record for the next workflow step.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> ApproveOrder(Guid id)
        {
            var result = await _volunteerOrderService.ApproveOrderAsync(id);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/reject")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Rejects the specified record according to administrative rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
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
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            await _volunteerOrderService.DeleteAsync(id);
            return NoContent();
        }
    }
}
