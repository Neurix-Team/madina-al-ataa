using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Persistance.Repositories
{
    public class DonationOrderRepository : IDonationOrderRepository
    {
        private readonly AppDbContext _context;

        public DonationOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // Implement CreateAsync method (change AddAsync to CreateAsync to match the interface)
        public async Task CreateAsync(DonationOrder donationOrder)
        {
            await _context.DonationOrders.AddAsync(donationOrder);
            await _context.SaveChangesAsync();
        }

        // Implement GetAllAsync with IEnumerable instead of List
        public async Task<IEnumerable<DonationOrder>> GetAllAsync()
        {
            return await _context.DonationOrders
                                 .Include(d => d.Donor)
                                 .Include(d => d.DonationRequest)
                                 .ToListAsync(); // still returning a List, but cast to IEnumerable
        }

        // Implement GetByIdAsync method
        public async Task<DonationOrder?> GetByIdAsync(Guid id)
        {
            return await _context.DonationOrders
                                 .Include(d => d.Donor)
                                 .Include(d => d.DonationRequest)
                                 .FirstOrDefaultAsync(d => d.Id == id);
        }

        // Implement UpdateAsync method
        public async Task UpdateAsync(DonationOrder donationOrder)
        {
            _context.DonationOrders.Update(donationOrder);
            await _context.SaveChangesAsync();
        }

    }
}