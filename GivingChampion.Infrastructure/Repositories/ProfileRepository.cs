using GivingChampion.API.Interfaces;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
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

        public async Task AddAsync(Profile profile)
        {
            await _context.Profiles.AddAsync(profile);
        }

        public void Update(Profile profile)
        {
            _context.Profiles.Update(profile);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}