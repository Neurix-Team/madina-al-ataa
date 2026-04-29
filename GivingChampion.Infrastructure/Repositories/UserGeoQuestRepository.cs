using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GivingChampion.API.Repositories
{
    public class UserGeoQuestRepository : IUserGeoQuestRepository
    {
        private readonly AppDbContext _context;

        public UserGeoQuestRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all UserGeoQuests with pagination
        public async Task<PagedList<UserGeoQuest>> GetAllByUserIdAsync(Guid userId,PageParameters pageParameters)
        {
            var query = _context.UserGeoQuests
                .Include(ugq => ugq.GeoQuest)
                .Include(ugq => ugq.User)
                .Where(ugq =>
                    ugq.UserId == userId &&
                    !ugq.IsDeleted);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(ugq => ugq.CreatedAt)
                .Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize)
                .Take(pageParameters.PageSize)
                .ToListAsync();

            return new PagedList<UserGeoQuest>(
                items,
                totalCount,
                pageParameters.PageNumber,
                pageParameters.PageSize
            );
        }

        // Get a single UserGeoQuest by its ID
        public async Task<UserGeoQuest?> GetByIdAsync(Guid id)
        {
            return await _context.UserGeoQuests
                .Include(ugq => ugq.GeoQuest)
                .Include(ugq => ugq.User)
                .FirstOrDefaultAsync(ugq =>
                    ugq.Id == id &&
                    !ugq.IsDeleted);
        }

        public async Task<UserGeoQuest?> GetByUserIdAndGeoQuestIdAsync(Guid userId, Guid geoQuestId)
        {
            return await _context.UserGeoQuests
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.GeoQuestId == geoQuestId &&
                    !x.IsDeleted);
        }

        // Add a new UserGeoQuest to the database
        public async Task AddAsync(UserGeoQuest userGeoQuest)
        {
            await _context.UserGeoQuests.AddAsync(userGeoQuest);
        }

        // Update an existing UserGeoQuest in the database
        public void Update(UserGeoQuest userGeoQuest)
        {
            _context.UserGeoQuests.Update(userGeoQuest);
        }

        // Save changes to the database
        public async Task SaveChangesAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}