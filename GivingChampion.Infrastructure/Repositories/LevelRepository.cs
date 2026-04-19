using GivingChampion.API.Interfaces;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
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

        public async Task<List<Level>> GetAllAsync()
        {
            return await _context.Levels
                .AsNoTracking()
                .ToListAsync();
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
    }
}