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

        // GET api/UserLevels/my
        // Current logged-in user gets his own level from token
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLevel()
        {
            var userLevel = await _userLevelService.GetMyLevelAsync();

            return Ok(userLevel);
        }

        // GET api/UserLevels/admin/user/{userId}
        // Admin gets level by user id
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/user/{userId:guid}")]
        public async Task<IActionResult> GetByUserIdForAdmin(Guid userId)
        {
            var userLevel = await _userLevelService.GetByUserIdAsync(userId);

            return Ok(userLevel);
        }

        // PUT api/UserLevels/admin/{id}
        // Admin updates user level
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserLevelDto dto)
        {

            await _userLevelService.UpdateAsync(id, dto);

            return NoContent();
        }

        //// DELETE api/UserLevels/admin/{id}
        //// Admin soft deletes user level
        //[Authorize(Roles = "Admin")]
        //[HttpDelete("admin/{id:guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    await _userLevelService.SoftDeleteAsync(id);

        //    return NoContent();
        //}
    }
}