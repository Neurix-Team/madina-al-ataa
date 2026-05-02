using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
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
                .ToListAsync();
        }

        public async Task<Profile?> GetByIdAsync(Guid id)
        {
            return await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Profile?> AddAsync(Guid userId, Guid levelId)
        {
            var profile = new Profile()
            {
                UserId = userId,
                LevelId = levelId
            };
            await _context.Profiles.AddAsync(profile);
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
                 .FirstOrDefaultAsync(p => p.UserId == id);
        }
    }
}
