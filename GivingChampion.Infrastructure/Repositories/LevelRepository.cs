using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Repositories
{
    public class LevelRepository : ILevelRepository
    {
        private readonly AppDbContext _context;

        public LevelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Level>> GetAllAsync(PageParameters pageParameters)
        {
            var query = _context.Levels.AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize)
                .Take(pageParameters.PageSize).ToPagedListAsync(pageParameters);

            return items;
        }

        public async Task<Level?> GetByIdAsync(Guid id)
        {
            return await _context.Levels
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(Level level)
        {
            await _context.Levels.AddAsync(level);
        }

        public void Update(Level level)
        {
            _context.Levels.Update(level);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Level?> GetFirstLevelAsync()
        {
            return await _context.Levels
                .OrderBy(l => l.Number)
                .FirstOrDefaultAsync();
        }
    }
}