using GivingChampion.Common.DTO.UserLevelDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserLevelsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserLevelsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userLevels = await _context.UserLevels
                .AsNoTracking()
                .Select(ul => new UserLevelDto
                {
                    Id = ul.Id,
                    Xp = ul.Xp,
                    Kp = ul.Kp,
                    ProfileId = ul.ProfileId,
                    LevelId = ul.LevelId,
                    LevelNumber = ul.Level.Number,
                    LevelMaxXp = ul.Level.MaxXp
                })
                .ToListAsync();

            return Ok(userLevels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userLevel = await _context.UserLevels
                .AsNoTracking()
                .Where(ul => ul.Id == id)
                .Select(ul => new UserLevelDto
                {
                    Id = ul.Id,
                    Xp = ul.Xp,
                    Kp = ul.Kp,
                    ProfileId = ul.ProfileId,
                    LevelId = ul.LevelId,
                    LevelNumber = ul.Level.Number,
                    LevelMaxXp = ul.Level.MaxXp
                })
                .FirstOrDefaultAsync();

            if (userLevel == null)
                return NotFound("User level not found");

            return Ok(userLevel);
        }

        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetByProfileId(Guid profileId)
        {
            var profileExists = await _context.Profiles
                .AnyAsync(p => p.Id == profileId);

            if (!profileExists)
                return NotFound("Profile not found");

            var userLevels = await _context.UserLevels
                .AsNoTracking()
                .Where(ul => ul.ProfileId == profileId)
                .Select(ul => new UserLevelDto
                {
                    Id = ul.Id,
                    Xp = ul.Xp,
                    Kp = ul.Kp,
                    ProfileId = ul.ProfileId,
                    LevelId = ul.LevelId,
                    LevelNumber = ul.Level.Number,
                    LevelMaxXp = ul.Level.MaxXp
                })
                .ToListAsync();

            return Ok(userLevels);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserLevelDto dto)
        {
            var profileExists = await _context.Profiles
                .AnyAsync(p => p.Id == dto.ProfileId);

            if (!profileExists)
                return BadRequest("Profile not found");

            var levelExists = await _context.Levels
                .AnyAsync(l => l.Id == dto.LevelId);

            if (!levelExists)
                return BadRequest("Level not found");

            var userLevel = new UserLevel
            {
                Xp = dto.Xp,
                Kp = dto.Kp,
                ProfileId = dto.ProfileId,
                LevelId = dto.LevelId
            };

            await _context.UserLevels.AddAsync(userLevel);
            await _context.SaveChangesAsync();

            var result = new UserLevelDto
            {
                Id = userLevel.Id,
                Xp = userLevel.Xp,
                Kp = userLevel.Kp,
                ProfileId = userLevel.ProfileId,
                LevelId = userLevel.LevelId
            };

            return CreatedAtAction(nameof(GetById), new { id = userLevel.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserLevelDto dto)
        {
            var userLevel = await _context.UserLevels
                .FirstOrDefaultAsync(ul => ul.Id == id);

            if (userLevel == null)
                return NotFound("User level not found");

            var levelExists = await _context.Levels
                .AnyAsync(l => l.Id == dto.LevelId);

            if (!levelExists)
                return BadRequest("Level not found");

            userLevel.Xp = dto.Xp;
            userLevel.Kp = dto.Kp;
            userLevel.LevelId = dto.LevelId;

            await _context.SaveChangesAsync();

            return Ok("User level updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userLevel = await _context.UserLevels
                .FirstOrDefaultAsync(ul => ul.Id == id);

            if (userLevel == null)
                return NotFound("User level not found");

            userLevel.IsDeleted = true;
            userLevel.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("User level deleted successfully");
        }
    }
}