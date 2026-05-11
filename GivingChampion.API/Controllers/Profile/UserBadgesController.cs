using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.UserBadge;
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

        // GET api/UserBadges
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userBadges = await _userBadgeService.GetAllByUserIdAsync();
            return Ok(userBadges);
        }

        // GET api/UserBadges/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userBadge = await _userBadgeService.GetByIdAsync(id);
            return Ok(userBadge);
        }

        // GET api/UserBadges/profile/{profileId}
        [Authorize(Roles = "Admin")]
        [HttpGet("profile/{profileId:guid}")]
        public async Task<IActionResult> GetByProfileId(Guid profileId)
        {
            var userBadges = await _userBadgeService.GetAllByProfileIdAsync(profileId);
            return Ok(userBadges);
        }

        // POST api/UserBadges
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserBadgeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _userBadgeService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        // PUT api/UserBadges/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserBadgeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _userBadgeService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound("User badge not found");

            return NoContent();
        }

        // DELETE api/UserBadges/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userBadgeService.SoftDeleteAsync(id);

            if (!deleted)
                return NotFound("User badge not found");

            return NoContent();
        }
    }
}
