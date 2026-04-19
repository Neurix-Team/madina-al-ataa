using GivingChampion.Common.DTO.BadgeDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BadgesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BadgesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var badges = await _context.Badges
                .AsNoTracking()
                .Select(b => new BadgeDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    Requirement = b.Requirement,
                    Category = b.Category
                })
                .ToListAsync();

            return Ok(badges);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var badge = await _context.Badges
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BadgeDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    Requirement = b.Requirement,
                    Category = b.Category
                })
                .FirstOrDefaultAsync();

            if (badge == null)
                return NotFound("Badge not found");

            return Ok(badge);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBadgeDto dto)
        {
            var badge = new Badge
            {
                Name = dto.Name,
                Description = dto.Description,
                Requirement = dto.Requirement,
                Category = dto.Category
            };

            await _context.Badges.AddAsync(badge);
            await _context.SaveChangesAsync();

            var result = new BadgeDto
            {
                Id = badge.Id,
                Name = badge.Name,
                Description = badge.Description,
                Requirement = badge.Requirement,
                Category = badge.Category
            };

            return CreatedAtAction(nameof(GetById), new { id = badge.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBadgeDto dto)
        {
            var badge = await _context.Badges
                .FirstOrDefaultAsync(b => b.Id == id);

            if (badge == null)
                return NotFound("Badge not found");

            badge.Name = dto.Name;
            badge.Description = dto.Description;
            badge.Requirement = dto.Requirement;
            badge.Category = dto.Category;

            await _context.SaveChangesAsync();

            return Ok("Badge updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var badge = await _context.Badges
                .FirstOrDefaultAsync(b => b.Id == id);

            if (badge == null)
                return NotFound("Badge not found");

            badge.IsDeleted = true;
            badge.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Badge deleted successfully");
        }
    }
}