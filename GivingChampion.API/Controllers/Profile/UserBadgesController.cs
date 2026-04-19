using GivingChampion.Common.DTO.UserBadgeDto;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserBadgesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserBadgesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userBadges = await _context.UserBadges
                .AsNoTracking()
                .Select(ub => new UserBadgeDto
                {
                    Id = ub.Id,
                    ProfileId = ub.ProfileId,
                    BadgeId = ub.BadgeId,
                    BadgeName = ub.Badge.Name,
                    BadgeCategory = ub.Badge.Category,
                    EarnedAt = ub.EarnedAt
                })
                .ToListAsync();

            return Ok(userBadges);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userBadge = await _context.UserBadges
                .AsNoTracking()
                .Where(ub => ub.Id == id)
                .Select(ub => new UserBadgeDto
                {
                    Id = ub.Id,
                    ProfileId = ub.ProfileId,
                    BadgeId = ub.BadgeId,
                    BadgeName = ub.Badge.Name,
                    BadgeCategory = ub.Badge.Category,
                    EarnedAt = ub.EarnedAt
                })
                .FirstOrDefaultAsync();

            if (userBadge == null)
                return NotFound("User badge not found");

            return Ok(userBadge);
        }

        [HttpGet("profile/{profileId}")]
        public async Task<IActionResult> GetByProfileId(Guid profileId)
        {
            var profileExists = await _context.Profiles
                .AnyAsync(p => p.Id == profileId);

            if (!profileExists)
                return NotFound("Profile not found");

            var userBadges = await _context.UserBadges
                .AsNoTracking()
                .Where(ub => ub.ProfileId == profileId)
                .Select(ub => new UserBadgeDto
                {
                    Id = ub.Id,
                    ProfileId = ub.ProfileId,
                    BadgeId = ub.BadgeId,
                    BadgeName = ub.Badge.Name,
                    BadgeCategory = ub.Badge.Category,
                    EarnedAt = ub.EarnedAt
                })
                .ToListAsync();

            return Ok(userBadges);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserBadgeDto dto)
        {
            var profileExists = await _context.Profiles
                .AnyAsync(p => p.Id == dto.ProfileId);

            if (!profileExists)
                return BadRequest("Profile not found");

            var badgeExists = await _context.Badges
                .AnyAsync(b => b.Id == dto.BadgeId);

            if (!badgeExists)
                return BadRequest("Badge not found");

            var existingUserBadge = await _context.UserBadges
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(ub =>
                    ub.ProfileId == dto.ProfileId &&
                    ub.BadgeId == dto.BadgeId);

            if (existingUserBadge != null)
            {
                if (!existingUserBadge.IsDeleted)
                    return BadRequest("This profile already has this badge");

                existingUserBadge.IsDeleted = false;
                existingUserBadge.DeletedAt = null;
                existingUserBadge.EarnedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var restoredResult = new UserBadgeDto
                {
                    Id = existingUserBadge.Id,
                    ProfileId = existingUserBadge.ProfileId,
                    BadgeId = existingUserBadge.BadgeId,
                    EarnedAt = existingUserBadge.EarnedAt
                };

                return Ok(restoredResult);
            }

            var userBadge = new UserBadge
            {
                ProfileId = dto.ProfileId,
                BadgeId = dto.BadgeId,
                EarnedAt = DateTime.UtcNow
            };

            await _context.UserBadges.AddAsync(userBadge);
            await _context.SaveChangesAsync();

            var result = new UserBadgeDto
            {
                Id = userBadge.Id,
                ProfileId = userBadge.ProfileId,
                BadgeId = userBadge.BadgeId,
                EarnedAt = userBadge.EarnedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = userBadge.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserBadgeDto dto)
        {
            var userBadge = await _context.UserBadges
                .FirstOrDefaultAsync(ub => ub.Id == id);

            if (userBadge == null)
                return NotFound("User badge not found");

            var badgeExists = await _context.Badges
                .AnyAsync(b => b.Id == dto.BadgeId);

            if (!badgeExists)
                return BadRequest("Badge not found");

            var duplicateExists = await _context.UserBadges
                .AnyAsync(ub =>
                    ub.Id != id &&
                    ub.ProfileId == userBadge.ProfileId &&
                    ub.BadgeId == dto.BadgeId);

            if (duplicateExists)
                return BadRequest("This profile already has this badge");

            userBadge.BadgeId = dto.BadgeId;

            await _context.SaveChangesAsync();

            return Ok("User badge updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userBadge = await _context.UserBadges
                .FirstOrDefaultAsync(ub => ub.Id == id);

            if (userBadge == null)
                return NotFound("User badge not found");

            userBadge.IsDeleted = true;
            userBadge.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("User badge deleted successfully");
        }
    }
}