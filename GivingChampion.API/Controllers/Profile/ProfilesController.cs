using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.ProfileDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET api/profiles/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound("Profile not found");

            return Ok(profile);
        }

        // POST api/profiles
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileDto dto)
        {
            try
            {
                var created = await _profileService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return error message if user/profile is not found
            }
        }

        // PUT api/profiles/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileDto dto)
        {
            var updated = await _profileService.UpdateAsync(id, dto);
            if (!updated) return NotFound("Profile not found");

            return NoContent();
        }

        // DELETE api/profiles/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _profileService.SoftDeleteAsync(id);
            if (!deleted) return NotFound("Profile not found");

            return NoContent();
        }
    }
}