using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.API.Repositories
{
    public class AiAvatarRepository : IAiAvatarRepository
    {
        private readonly AppDbContext _context;

        public AiAvatarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AiAvatar>> GetAllAsync()
        {
            return await _context.AiAvatars
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AiAvatar?> GetByIdAsync(Guid id)
        {
            return await _context.AiAvatars
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(AiAvatar aiAvatar)
        {
            await _context.AiAvatars.AddAsync(aiAvatar);
        }

        public void Update(AiAvatar aiAvatar)
        {
            _context.AiAvatars.Update(aiAvatar);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}