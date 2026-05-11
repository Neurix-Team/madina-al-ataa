using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace GivingChampion.API.Repositories
{
    public class GeoQuestRepository : IGeoQuestRepository
    {
        private readonly AppDbContext _context;

        public GeoQuestRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all GeoQuests with pagination
        public async Task<PagedList<GeoQuest>> GetAllAsync(Guid userId, PageParameters pageParameters)
        {
            var userGeoQuestIds = _context.UserGeoQuests
                .Where(ugq => ugq.UserId == userId)  
                .Select(ugq => ugq.GeoQuestId)
                .ToList();
            var geoQuests = await _context.GeoQuests
                .AsNoTracking()  // For read-only operation, improving performance
                .Where(gq => !userGeoQuestIds.Contains(gq.Id) && !gq.IsDeleted)
                .ToPagedListAsync(pageParameters);
            return geoQuests;
        }

        // Get a single GeoQuest by its ID
        public async Task<GeoQuest?> GetByIdAsync(Guid id)
        {
            return await _context.GeoQuests
                .Include(gq => gq.Location)
                .FirstOrDefaultAsync(gq =>
                    gq.Id == id &&
                    !gq.IsDeleted);
        }

        // Add a new GeoQuest to the database
        public async Task AddAsync(GeoQuest geoQuest)
        {
            await _context.GeoQuests.AddAsync(geoQuest);
        }

        // Update an existing GeoQuest in the database
        public void Update(GeoQuest geoQuest)
        {
            _context.GeoQuests.Update(geoQuest);
        }

    }
}
