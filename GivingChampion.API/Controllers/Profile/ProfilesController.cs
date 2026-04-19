using GivingChampion.Common.DTO.ProfileDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _context.Profiles
                .AsNoTracking()
                .Select(p => new ProfileDto
                {
                    Id = p.Id,
                    Rating = p.Rating,
                    Impact = p.Impact,
                    UserId = p.UserId,
                    AvatarId = p.AvatarId,
                    LevelId = p.LevelId,
                    AvatarName = p.Avatar.CharacterName,
                    LevelNumber = p.Level.Number
                })
                .ToListAsync();

            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profile = await _context.Profiles
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProfileDto
                {
                    Id = p.Id,
                    Rating = p.Rating,
                    Impact = p.Impact,
                    UserId = p.UserId,
                    AvatarId = p.AvatarId,
                    LevelId = p.LevelId,
                    AvatarName = p.Avatar.CharacterName,
                    LevelNumber = p.Level.Number
                })
                .FirstOrDefaultAsync();

            if (profile == null)
                return NotFound("Profile not found");

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileDto dto)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == dto.UserId);

            if (!userExists)
                return BadRequest("User not found");

            var avatarExists = await _context.Avatars
                .AnyAsync(a => a.Id == dto.AvatarId);

            if (!avatarExists)
                return BadRequest("Avatar not found");

            var levelExists = await _context.Levels
                .AnyAsync(l => l.Id == dto.LevelId);

            if (!levelExists)
                return BadRequest("Level not found");

            var userAlreadyHasProfile = await _context.Profiles
                .AnyAsync(p => p.UserId == dto.UserId);

            if (userAlreadyHasProfile)
                return BadRequest("This user already has a profile");

            var profile = new Domain.Entities.Profile
            {
                Rating = dto.Rating,
                Impact = dto.Impact,
                UserId = dto.UserId,
                AvatarId = dto.AvatarId,
                LevelId = dto.LevelId
            };

            await _context.Profiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            var result = new ProfileDto
            {
                Id = profile.Id,
                Rating = profile.Rating,
                Impact = profile.Impact,
                UserId = profile.UserId,
                AvatarId = profile.AvatarId,
                LevelId = profile.LevelId
            };

            return CreatedAtAction(nameof(GetById), new { id = profile.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileDto dto)
        {
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
                return NotFound("Profile not found");

            var avatarExists = await _context.Avatars
                .AnyAsync(a => a.Id == dto.AvatarId);

            if (!avatarExists)
                return BadRequest("Avatar not found");

            var levelExists = await _context.Levels
                .AnyAsync(l => l.Id == dto.LevelId);

            if (!levelExists)
                return BadRequest("Level not found");

            profile.Rating = dto.Rating;
            profile.Impact = dto.Impact;
            profile.AvatarId = dto.AvatarId;
            profile.LevelId = dto.LevelId;

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
                return NotFound("Profile not found");

            profile.IsDeleted = true;
            profile.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Profile deleted successfully");
        }
    }
}