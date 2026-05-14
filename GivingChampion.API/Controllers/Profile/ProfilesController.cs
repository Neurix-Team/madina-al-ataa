using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.ProfileDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// Handles HTTP requests for Profiles.
    /// </summary>
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        /// <summary>
        /// Performs the ProfilesController operation.
        /// </summary>
        /// <param name="profileService">Provides the profileService value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Authorize]
        [HttpGet("{id}")]
        /// <summary>
        /// Retrieves a single record by its identifier.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound("Profile not found");

            return Ok(profile);
        }

        [Authorize]
        [HttpGet("my")]
        /// <summary>
        /// Performs the GetByUserId operation.
        /// </summary>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> GetByUserId()
        {
            var profile = await _profileService.GetByUserIdAsync();
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("{id}")]
        /// <summary>
        /// Updates an existing record from the supplied request data.
        /// </summary>
        /// <param name="id">Provides the id value required by the operation.</param>
        /// <param name="dto">Provides the dto value required by the operation.</param>
        /// <returns>The result produced by the operation.</returns>
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileDto dto)
        {
            var updated = await _profileService.UpdateAsync(id, dto);
            if (!updated) return NotFound("Profile not found");

            return NoContent();
        }
    }
}
