using GivingChampion.API.Interfaces;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Repositories
{
    public class UserBadgeRepository : IUserBadgeRepository
    {
        private readonly AppDbContext _context;

        public UserBadgeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserBadge>> GetAllAsync()
        {
            return await _context.UserBadges
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserBadge?> GetByIdAsync(Guid id)
        {
            return await _context.UserBadges
                .FirstOrDefaultAsync(ub => ub.Id == id);
        }

        public async Task AddAsync(UserBadge userBadge)
        {
            await _context.UserBadges.AddAsync(userBadge);
        }

        public void Update(UserBadge userBadge)
        {
            _context.UserBadges.Update(userBadge);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}