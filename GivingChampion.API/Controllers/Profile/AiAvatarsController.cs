using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.AiAvatarDto;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
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
        [HttpGet("{id}")]
        public async Task<ActionResult<AiAvatarDto>> GetById(Guid id)
        {
            var aiAvatar = await _aiAvatarService.GetByIdAsync(id);
            if (aiAvatar == null)
                return NotFound();
            return Ok(aiAvatar);
        }

        // POST api/aiavatars
        [HttpPost]
        public async Task<ActionResult<AiAvatarDto>> Create([FromBody] CreateAiAvatarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _aiAvatarService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/aiavatars/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAiAvatarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _aiAvatarService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE api/aiavatars/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _aiAvatarService.SoftDeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}