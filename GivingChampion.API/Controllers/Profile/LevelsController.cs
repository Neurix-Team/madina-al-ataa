using GivingChampion.Common.DTO.LevelDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LevelsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var levels = await _context.Levels
                .AsNoTracking()
                .Select(l => new LevelDto
                {
                    Id = l.Id,
                    Number = l.Number,
                    MaxXp = l.MaxXp
                })
                .ToListAsync();

            return Ok(levels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var level = await _context.Levels
                .AsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => new LevelDto
                {
                    Id = l.Id,
                    Number = l.Number,
                    MaxXp = l.MaxXp
                })
                .FirstOrDefaultAsync();

            if (level == null)
                return NotFound("Level not found");

            return Ok(level);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLevelDto dto)
        {
            var level = new Level
            {
                Number = dto.Number,
                MaxXp = dto.MaxXp
            };

            await _context.Levels.AddAsync(level);
            await _context.SaveChangesAsync();

            var result = new LevelDto
            {
                Id = level.Id,
                Number = level.Number,
                MaxXp = level.MaxXp
            };

            return CreatedAtAction(nameof(GetById), new { id = level.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLevelDto dto)
        {
            var level = await _context.Levels
                .FirstOrDefaultAsync(l => l.Id == id);

            if (level == null)
                return NotFound("Level not found");

            level.Number = dto.Number;
            level.MaxXp = dto.MaxXp;

            await _context.SaveChangesAsync();

            return Ok("Level updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var level = await _context.Levels
                .FirstOrDefaultAsync(l => l.Id == id);

            if (level == null)
                return NotFound("Level not found");

            level.IsDeleted = true;
            level.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Level deleted successfully");
        }
    }
}