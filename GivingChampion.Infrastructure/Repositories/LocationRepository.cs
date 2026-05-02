using GivingChampion.Application.Interfaces;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Infrastructure.Persistence.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> GetByIdAsync(Guid id)
        {
            return await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _context.Locations
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.RequiredLevel)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<List<Location>> GetAvailableForLevelAsync(int userLevel)
        {
            return await _context.Locations
                .Where(l => l.RequiredLevel <= userLevel && !l.IsDeleted)
                .OrderBy(l => l.RequiredLevel)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }

        public async Task CreateAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
        }

        public Task UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            return Task.CompletedTask;
        }

        public async Task SoftDeleteAsync(Guid locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            if (location == null) return;

            //location.IsDeleted = true;
            //location.DeletedAt = DateTime.UtcNow;

            _context.Locations.Remove(location);
        }
    }
}
