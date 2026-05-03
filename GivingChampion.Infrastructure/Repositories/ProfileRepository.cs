using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Profile>> GetAllAsync()
        {
            return await _context.Profiles
                .AsNoTracking()
                .Include(p => p.Level)
                .Include(p => p.UserLevel)
                    .ThenInclude(ul => ul.Level)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Profile?> GetByIdAsync(Guid id)
        {
            return await _context.Profiles
                .Include(p => p.Level)
                .Include(p => p.UserLevel)
                    .ThenInclude(ul => ul.Level)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Profile?> AddAsync(Guid userId, Guid levelId)
        {
            var selectedLevelId = levelId;

            if (selectedLevelId == Guid.Empty)
            {
                selectedLevelId = await _context.Levels
                    .Where(l => !l.IsDeleted)
                    .OrderBy(l => l.Number)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync();
            }

            if (selectedLevelId == Guid.Empty)
                throw new InvalidOperationException("Default level was not found.");

            var profile = new Profile
            {
                UserId = userId,
                LevelId = selectedLevelId,
                CreatedAt = DateTime.UtcNow
            };

            var userLevel = new UserLevel
            {
                Profile = profile,
                LevelId = selectedLevelId,
                Xp = 0,
                Kp = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Profiles.AddAsync(profile);
            await _context.UserLevels.AddAsync(userLevel);

            return profile;
        }

        public Task Update(Profile profile)
        {
            _context.Profiles.Update(profile);
            return Task.CompletedTask;
        }

        public async Task<Profile?> GetByUserIdAsync(Guid id)
        {
            return await _context.Profiles
                .Include(p => p.Level)
                .Include(p => p.UserLevel)
                    .ThenInclude(ul => ul.Level)
                .FirstOrDefaultAsync(p => p.UserId == id && !p.IsDeleted);
        }
    }
}