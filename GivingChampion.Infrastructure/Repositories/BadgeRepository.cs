using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Repositories
{
    public class BadgeRepository : IBadgeRepository
    {
        private readonly AppDbContext _context;

        public BadgeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Badge>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.Badges
                .AsNoTracking().ToPagedListAsync(pageParameters);
        }

        public async Task<Badge?> GetByIdAsync(Guid id)
        {
            return await _context.Badges
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Badge badge)
        {
            await _context.Badges.AddAsync(badge);
        }

        public void Update(Badge badge)
        {
            _context.Badges.Update(badge);
        }

    }
}
