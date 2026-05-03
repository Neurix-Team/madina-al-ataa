using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.ProfileDto;
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

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound("Profile not found");

            return Ok(profile);
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetByUserId()
        {
            var profile = await _profileService.GetByUserIdAsync();
            return Ok(profile);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileDto dto)
        {
            var updated = await _profileService.UpdateAsync(id, dto);
            if (!updated) return NotFound("Profile not found");

            return NoContent();
        }
    }
}
