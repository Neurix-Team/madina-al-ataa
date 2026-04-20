using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.UserBadgeDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserBadgesController : ControllerBase
    {
        private readonly IUserBadgeService _userBadgeService;

        public UserBadgesController(IUserBadgeService userBadgeService)
        {
            _userBadgeService = userBadgeService;
        }

        // GET api/userbadges
        [HttpGet]
        public async Task<IActionResult> GetAll(Guid profileId)
        {
            var userBadges = await _userBadgeService.GetAllByProfileIdAsync(profileId);
            return Ok(userBadges);
        }

        // GET api/userbadges/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userBadge = await _userBadgeService.GetByIdAsync(id);
            if (userBadge == null)
                return NotFound("User badge not found");

            return Ok(userBadge);
        }

        // GET api/userbadges/profile/{profileId}
        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetByProfileId(Guid profileId)
        {
            var userBadges = await _userBadgeService.GetByIdAsync(profileId);
            return Ok(userBadges);
        }

        // POST api/userbadges
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserBadgeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _userBadgeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/userbadges/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserBadgeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _userBadgeService.UpdateAsync(id, dto);
            if (!updated) return NotFound("User badge not found");
            return NoContent();
        }

        // DELETE api/userbadges/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userBadgeService.SoftDeleteAsync(id);
            if (!deleted) return NotFound("User badge not found");
            return NoContent();
        }
    }
}