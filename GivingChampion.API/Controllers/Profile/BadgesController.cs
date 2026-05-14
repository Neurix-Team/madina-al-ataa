using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.BadgeDto;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Handles HTTP requests for Badges.
    /// </summary>
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        /// <summary>
        /// Performs the BadgesController operation.
        /// </summary>
        /// <param name="badgeService">Provides the badgeService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        [HttpGet]
        /// <summary>
        /// Retrieves a paged collection of records that match the request.
        /// </summary>
        /// <param name="pageParameters">Provides the pageParameters value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var badges = await _badgeService.GetAllAsync(pageParameters);
            return Ok(badges);
        }

        [HttpGet("{id:guid}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var badge = await _badgeService.GetByIdAsync(id);
            return Ok(badge);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Create([FromBody] CreateBadgeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _badgeService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Value!.Id },
                created
            );
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBadgeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _badgeService.UpdateAsync(id, dto);

            if (!updated.Value)
                return NotFound("Badge not found");

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _badgeService.SoftDeleteAsync(id);

            if (!deleted.Value)
                return NotFound("Badge not found");

            return NoContent();
        }
    }
}
