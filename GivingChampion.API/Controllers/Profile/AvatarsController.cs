using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.AvatarDto;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvatarsController : ControllerBase
    {
        private readonly IAvatarService _avatarService;

        public AvatarsController(IAvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var avatars = await _avatarService.GetAllAsync();
        //    return Ok(avatars);
        //}

        // GET api/avatars/{id}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var avatar = await _avatarService.GetByIdAsync(id);

            if (avatar == null)
                return NotFound("Avatar not found");

            return Ok(avatar);
        }
        // POST api/avatars
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAvatarDto dto)
        {
            var createdAvatar = await _avatarService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdAvatar.Id }, createdAvatar);
        }
        // PUT api/avatars/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAvatarDto dto)
        {
            var updated = await _avatarService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound("Avatar not found");

            return Ok(new { message = "Avatar updated successfully" });
        }
        // DELETE api/avatars/{id}
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