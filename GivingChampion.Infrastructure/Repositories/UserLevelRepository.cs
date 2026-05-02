using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Repositories
{
    public class UserLevelRepository : IUserLevelRepository
    {
        private readonly AppDbContext _context;

        public UserLevelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserLevel>> GetAllAsync()
        {
            return await _context.UserLevels
                .AsNoTracking()
                .Include(ul => ul.Level) // Include Level entity for level name
                .ToListAsync();
        }

        public async Task<UserLevel?> GetByProfileIdAsync(Guid profileId)
        {
            return await _context.UserLevels
                .Include(ul => ul.Level) // Include Level entity for level name
                .FirstOrDefaultAsync(ul => ul.ProfileId == profileId);
        }

        public async Task AddAsync(UserLevel userLevel)
        {
            await _context.UserLevels.AddAsync(userLevel);
        }

        public void Update(UserLevel userLevel)
        {
            _context.UserLevels.Update(userLevel);
        }

        public async Task<UserLevel?> GetByIdAsync(Guid id)
        {
            return await _context.UserLevels
                .Include(ul => ul.Level) // Include Level entity for level name
                .FirstOrDefaultAsync(ul => ul.Id == id);
        }
    }
}
