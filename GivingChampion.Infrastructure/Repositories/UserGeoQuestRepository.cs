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
        public async Task<List<UserGeoQuest>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.UserGeoQuests
                .AsNoTracking() // For read-only operation, improving performance
                .Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize) // Pagination logic
                .Take(pageParameters.PageSize) // Pagination logic
                .Include(ugq => ugq.GeoQuest) // Include the GeoQuest related data
                .Include(ugq => ugq.User) // Include User related data
                .ToListAsync();
        }

        // Get a single UserGeoQuest by its ID
        public async Task<UserGeoQuest?> GetByIdAsync(Guid id)
        {
            return await _context.UserGeoQuests
                .AsNoTracking()
                .Include(ugq => ugq.GeoQuest)
                .Include(ugq => ugq.User)
                .FirstOrDefaultAsync(ugq => ugq.Id == id);
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