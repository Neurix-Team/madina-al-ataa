using GivingChampion.Common.DTO.AiAvatarDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiAvatarsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AiAvatarsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var aiAvatars = await _context.AiAvatars
                .AsNoTracking()
                .Select(a => new AiAvatarDto
                {
                    Id = a.Id,
                    FavoriteCategory = a.FavoriteCategory,
                    SuccessRate = a.SuccessRate,
                    LastSuggestion = a.LastSuggestion
                })
                .ToListAsync();

            return Ok(aiAvatars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var aiAvatar = await _context.AiAvatars
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AiAvatarDto
                {
                    Id = a.Id,
                    FavoriteCategory = a.FavoriteCategory,
                    SuccessRate = a.SuccessRate,
                    LastSuggestion = a.LastSuggestion
                })
                .FirstOrDefaultAsync();

            if (aiAvatar == null)
                return NotFound("AiAvatar not found");

            return Ok(aiAvatar);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAiAvatarDto dto)
        {
            var aiAvatar = new AiAvatar
            {
                FavoriteCategory = dto.FavoriteCategory,
                SuccessRate = dto.SuccessRate,
                LastSuggestion = dto.LastSuggestion
            };

            await _context.AiAvatars.AddAsync(aiAvatar);
            await _context.SaveChangesAsync();

            var result = new AiAvatarDto
            {
                Id = aiAvatar.Id,
                FavoriteCategory = aiAvatar.FavoriteCategory,
                SuccessRate = aiAvatar.SuccessRate,
                LastSuggestion = aiAvatar.LastSuggestion
            };

            return CreatedAtAction(nameof(GetById), new { id = aiAvatar.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAiAvatarDto dto)
        {
            var aiAvatar = await _context.AiAvatars
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aiAvatar == null)
                return NotFound("AiAvatar not found");

            aiAvatar.FavoriteCategory = dto.FavoriteCategory;
            aiAvatar.SuccessRate = dto.SuccessRate;
            aiAvatar.LastSuggestion = dto.LastSuggestion;

            await _context.SaveChangesAsync();

            return Ok("AiAvatar updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var aiAvatar = await _context.AiAvatars
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aiAvatar == null)
                return NotFound("AiAvatar not found");

            aiAvatar.IsDeleted = true;
            aiAvatar.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("AiAvatar deleted successfully");
        }
    }
}