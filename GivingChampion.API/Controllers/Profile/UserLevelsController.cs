using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.UserLevelDto;
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

        // GET api/userlevels
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userLevel = await _userLevelService.GetByProfileIdAsync(id);
            if (userLevel == null)
                return NotFound("User level not found");

            return Ok(userLevel);
        }

        //// POST api/userlevels
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateUserLevelDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var created = await _userLevelService.CreateAsync(dto);
        //    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        //}

        // PUT api/userlevels/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserLevelDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _userLevelService.UpdateAsync(id, dto);
            if (!updated) return NotFound("User level not found");
            return NoContent();
        }

        // DELETE api/userlevels/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userLevelService.SoftDeleteAsync(id);
            if (!deleted) return NotFound("User level not found");
            return NoContent();
        }
    }
}