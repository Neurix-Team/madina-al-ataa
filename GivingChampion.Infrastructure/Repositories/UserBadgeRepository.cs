using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
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

        public async Task<List<UserBadge>> GetAllByProfileIdAsync(Guid profileId)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .Include(ub => ub.Badge)
                .Include(ub => ub.Profile)
                .Where(ub => ub.ProfileId == profileId && !ub.IsDeleted)
                .ToListAsync();
        }

        public async Task<UserBadge?> GetByIdAsync(Guid id)
        {
            return await _context.UserBadges
                .Include(ub => ub.Badge)
                .Include(ub => ub.Profile)
                .FirstOrDefaultAsync(ub => ub.Id == id && !ub.IsDeleted);
        }

        public async Task AddAsync(UserBadge userBadge)
        {
            await _context.UserBadges.AddAsync(userBadge);
        }

        public void Update(UserBadge userBadge)
        {
            _context.UserBadges.Update(userBadge);
        }

        public Task DeleteAsync(UserBadge userBadge)
        {
            _context.UserBadges.Update(userBadge);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<UserBadge?> GetByProfileAndBadgeAsync(
            Guid profileId,
            Guid badgeId,
            bool includeDeleted = false)
        {
            var query = _context.UserBadges
                .Include(ub => ub.Badge)
                .Include(ub => ub.Profile)
                .AsQueryable();

            if (includeDeleted)
                query = query.IgnoreQueryFilters();

            return await query.FirstOrDefaultAsync(ub =>
                ub.ProfileId == profileId &&
                ub.BadgeId == badgeId);
        }
    }
}