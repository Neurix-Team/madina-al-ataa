using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.AiAvatarDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Handles HTTP requests for AiAvatars.
    /// </summary>
    public class AiAvatarsController : ControllerBase
    {
        private readonly IAiAvatarService _aiAvatarService;

        /// <summary>
        /// Performs the AiAvatarsController operation.
        /// </summary>
        /// <param name="aiAvatarService">Provides the aiAvatarService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public AiAvatarsController(IAiAvatarService aiAvatarService)
        {
            _aiAvatarService = aiAvatarService;
        }

        // GET api/aiavatars/{id}
        [Authorize]
        [HttpGet("{id}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<AiAvatarDto>> GetById(Guid id)
        {
            var aiAvatar = await _aiAvatarService.GetByIdAsync(id);
            if (aiAvatar == null)
                return NotFound();
            return Ok(aiAvatar);
        }

        // POST api/aiavatars
        [Authorize]
        [HttpPost]
        /// <summary>
        /// Creates a new record from the supplied request data.
        /// </summary>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<ActionResult<AiAvatarDto>> Create([FromBody] CreateAiAvatarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _aiAvatarService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/aiavatars/{id}
        [Authorize]
        [HttpPut("{id}")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAiAvatarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _aiAvatarService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE api/aiavatars/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        /// <summary>
        /// Removes or deactivates the specified record according to business rules.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _aiAvatarService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
