using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Avatar> AddAsync(Guid profileId)
        {
            var avatar = new Avatar
            {
                ProfileId = profileId
            };
            await _context.Avatars.AddAsync(avatar);
            return avatar;
        }

        public void Update(Avatar avatar)
        {
            _context.Avatars.Update(avatar);
        }

        public async Task<Avatar?> GetByProfileIdAsync(Guid id)
        {
            return await _context.Avatars
                .FirstOrDefaultAsync(a => a.ProfileId == id);
        }
    }
}
