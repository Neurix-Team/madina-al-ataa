using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.ActivityDto;
using GivingChampion.Common.DTO.GivingChampion.Common.DTO.ActivityDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GivingChampion.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // GET api/activities
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var activities = await _activityService.GetAllAsync();
            return Ok(activities);
        }

        // GET api/activities/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var activity = await _activityService.GetByIdAsync(id);
            if (activity == null)
                return NotFound("Activity not found");

            return Ok(activity);
        }

        // POST api/activities
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActivityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _activityService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/activities/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateActivityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _activityService.UpdateAsync(id, dto);
            if (!updated) return NotFound("Activity not found");
            return NoContent();
        }

        // DELETE api/activities/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _activityService.SoftDeleteAsync(id);
            if (!deleted) return NotFound("Activity not found");
            return NoContent();
        }
    }
}