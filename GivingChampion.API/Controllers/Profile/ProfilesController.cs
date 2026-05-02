using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.ProfileDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            if (userId == Guid.Empty)
                return Unauthorized("User ID not found in claims.");

            var profile = await _profileService.GetByUserIdAsync(userId);
            //if (profile == null)
            //    return NotFound("Profile not found");

            return Ok(profile);
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

        //// DELETE api/profiles/{id}
        //[Authorize]
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var deleted = await _profileService.SoftDeleteAsync(id);
        //    if (!deleted) return NotFound("Profile not found");

        //    return NoContent();
        //}
    }
}