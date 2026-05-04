using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using GivingChampion.Persistence.Contexts;
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
                .Include(ul => ul.Level)
                .Where(ul => !ul.IsDeleted)
                .ToListAsync();
        }

        public async Task<UserLevel?> GetByProfileIdAsync(Guid profileId)
        {
            return await _context.UserLevels
                .Include(ul => ul.Level)
                .FirstOrDefaultAsync(ul =>
                    ul.ProfileId == profileId &&
                    !ul.IsDeleted);
        }

        public async Task<UserLevel?> GetByIdAsync(Guid id)
        {
            return await _context.UserLevels
                .Include(ul => ul.Level)
                .FirstOrDefaultAsync(ul =>
                    ul.Id == id &&
                    !ul.IsDeleted);
        }

        public async Task AddAsync(UserLevel userLevel)
        {
            await _context.UserLevels.AddAsync(userLevel);
        }

        public void Update(UserLevel userLevel)
        {
            _context.UserLevels.Update(userLevel);
        }
    }
}