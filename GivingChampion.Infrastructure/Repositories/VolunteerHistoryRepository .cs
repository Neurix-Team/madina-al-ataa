using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;  // Add this for Entity Framework extension methods
using System;
using System.Collections.Generic;
using System.Linq;  // Ensure LINQ extension methods like Where, OrderBy are available
using System.Threading.Tasks;  // Ensure async methods are available

namespace GivingChampion.Persistance.Repositories
{
    public class VolunteerHistoryRepository : IVolunteerHistoryRepository
    {
        private readonly AppDbContext _context;

        public VolunteerHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VolunteerHistories history)
        {
            await _context.VolunteerHistories.AddAsync(history);
            await SaveChangesAsync();  // Ensure changes are saved after adding
        }

        public async Task<List<VolunteerHistories>> GetByUserIdAsync(Guid userId)
        {
            return await _context.VolunteerHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(); // ToListAsync() works correctly if Entity Framework is set up properly
        }

        public async Task<List<VolunteerHistories>> GetByRequestIdAsync(Guid requestId)
        {
            return await _context.VolunteerHistories
                .Where(x => x.ServiceRequestId == requestId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(); // ToListAsync() works correctly if Entity Framework is set up properly
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();  // Ensure changes are saved
        }
    }
}