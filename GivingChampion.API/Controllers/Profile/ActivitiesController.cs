using GivingChampion.Common.DTO.ActivityDto;
using GivingChampion.Common.DTO.GivingChampion.Common.DTO.ActivityDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var activities = await _context.Activities
                .AsNoTracking()
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var activity = await _context.Activities
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (activity == null)
                return NotFound("Activity not found");

            return Ok(activity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActivityDto dto)
        {
            var activity = new Activity
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Activities.AddAsync(activity);
            await _context.SaveChangesAsync();

            var result = new ActivityDto
            {
                Id = activity.Id,
                Name = activity.Name,
                Description = activity.Description,
                CreatedAt = activity.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = activity.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateActivityDto dto)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound("Activity not found");

            activity.Name = dto.Name;
            activity.Description = dto.Description;

            await _context.SaveChangesAsync();

            return Ok("Activity updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound("Activity not found");

            activity.IsDeleted = true;
            activity.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Activity deleted successfully");
        }
    }
}