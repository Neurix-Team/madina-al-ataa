using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.AvatarDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AvatarsController : ControllerBase
    {
        private readonly IAvatarService _avatarService;

        public AvatarsController(IAvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        // GET api/avatars/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var avatar = await _avatarService.GetByIdAsync(id);

            if (avatar == null)
                return NotFound("Avatar not found");

            return Ok(avatar);
        }

        // PUT api/avatars/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAvatarDto dto)
        {
            var updated = await _avatarService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound("Avatar not found");

            return Ok(new { message = "Avatar updated successfully" });
        }
        // DELETE api/avatars/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _avatarService.SoftDeleteAsync(id);

            if (!deleted)
                return NotFound("Avatar not found");

            return Ok(new { message = "Avatar deleted successfully" });
        }
    }
}