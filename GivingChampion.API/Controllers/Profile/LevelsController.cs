using GivingChampion.API.Interfaces;
using GivingChampion.Application.DTO.LevelDto;
using GivingChampion.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LevelsController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        // GET api/levels
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParameters pageParameters)
        {
            // Call service method to get all levels with pagination
            var levels = await _levelService.GetAllAsync(pageParameters);
            return Ok(levels); // Return levels as response
        }

        // GET api/levels/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Call service method to get a level by id
            var level = await _levelService.GetByIdAsync(id);
            if (level == null)
                return NotFound("Level not found");

            return Ok(level); // Return the level as response
        }

        // POST api/levels
        [Authorize (Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLevelDto dto)
        {
            // Call service method to create a level
            var created = await _levelService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Value.Id }, created); // Return created level
        }

        // PUT api/levels/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLevelDto dto)
        {
            // Call service method to update a level
            var updated = await _levelService.UpdateAsync(id, dto);
            if (!updated.Value) return NotFound("Level not found");

            return NoContent(); // Return NoContent status if updated successfully
        }

        // DELETE api/levels/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Call service method to soft delete a level
            var deleted = await _levelService.SoftDeleteAsync(id);
            if (!deleted.Value) return NotFound("Level not found");

            return NoContent(); // Return NoContent status if deleted successfully
        }
    }
}