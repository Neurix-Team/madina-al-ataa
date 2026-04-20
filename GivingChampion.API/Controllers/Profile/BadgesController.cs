using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.BadgeDto;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        // GET api/badges
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            var badges = await _badgeService.GetAllAsync(pageParameters);
            return Ok(badges);
        }

        // GET api/badges/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var badge = await _badgeService.GetByIdAsync(id);
            if (badge == null)
                return NotFound("Badge not found");

            return Ok(badge);
        }

        // POST api/badges
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBadgeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _badgeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Value!.Id }, created);
        }

        // PUT api/badges/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBadgeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _badgeService.UpdateAsync(id, dto);
            if (!updated.Value) return NotFound("Badge not found");
            return NoContent();
        }

        // DELETE api/badges/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _badgeService.SoftDeleteAsync(id);
            if (!deleted.Value) return NotFound("Badge not found");
            return NoContent();
        }
    }
}