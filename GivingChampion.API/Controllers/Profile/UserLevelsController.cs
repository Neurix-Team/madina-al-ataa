using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.UserLevelDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserLevelsController : ControllerBase
    {
        private readonly IUserLevelService _userLevelService;

        public UserLevelsController(IUserLevelService userLevelService)
        {
            _userLevelService = userLevelService;
        }

        // GET api/UserLevels/profile/{profileId}
        [HttpGet("profile/{profileId:guid}")]
        public async Task<IActionResult> GetByProfileId(Guid profileId)
        {
            var userLevel = await _userLevelService.GetByProfileIdAsync(profileId);
            return Ok(userLevel);
        }

        // PUT api/UserLevels/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserLevelDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _userLevelService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound("User level not found");

            return NoContent();
        }

        // DELETE api/UserLevels/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userLevelService.SoftDeleteAsync(id);

            if (!deleted)
                return NotFound("User level not found");

            return NoContent();
        }
    }
}