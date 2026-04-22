using GivingChampion.API.Interfaces;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<GeoQuest>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.GeoQuests
                .AsNoTracking()  // For read-only operation, improving performance
                .Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize)  // Pagination logic
                .Take(pageParameters.PageSize)  // Pagination logic
                .ToListAsync();
        }

        // Get a single GeoQuest by its ID
        public async Task<GeoQuest?> GetByIdAsync(Guid id)
        {
            return await _context.GeoQuests
                .AsNoTracking()
                .FirstOrDefaultAsync(gq => gq.Id == id);
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

        // Save changes to the database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}