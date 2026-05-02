using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.AiAvatarDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AiAvatarsController : ControllerBase
    {
        private readonly IAiAvatarService _aiAvatarService;

        public AiAvatarsController(IAiAvatarService aiAvatarService)
        {
            _aiAvatarService = aiAvatarService;
        }

        // GET api/aiavatars/{id}
        [Authorize]
        [HttpGet("{id}")]
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
        public async Task<ActionResult<AiAvatarDto>> Create([FromBody] CreateAiAvatarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _aiAvatarService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/aiavatars/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAiAvatarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _aiAvatarService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE api/aiavatars/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _aiAvatarService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}