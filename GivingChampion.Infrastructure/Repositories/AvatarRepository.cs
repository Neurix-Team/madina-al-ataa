using GivingChampion.API.Interfaces;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
//GivingChampion.Persistance
namespace GivingChampion.API.Repositories
{
    public class AvatarRepository : IAvatarRepository
    {
        private readonly AppDbContext _context;

        public AvatarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Avatar>> GetAllAsync()
        {
            return await _context.Avatars
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Avatar?> GetByIdAsync(Guid id)
        {
            return await _context.Avatars
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Avatar avatar)
        {
            await _context.Avatars.AddAsync(avatar);
        }

        public void Update(Avatar avatar)
        {
            _context.Avatars.Update(avatar);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}