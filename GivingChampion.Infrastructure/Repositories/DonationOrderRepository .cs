using GivingChampion.Common.Enums;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task CreateAsync(DonationOrder donationOrder)
        {
            await _context.DonationOrders.AddAsync(donationOrder);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DonationOrder>> GetAllAsync()
        {
            return await _context.DonationOrders
                .AsNoTracking()
                .Include(d => d.Donor)
                .Include(d => d.DonationRequest)
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<DonationOrder>> GetByDonorIdAsync(Guid donorUserId)
        {
            return await _context.DonationOrders
                .AsNoTracking()
                .Include(d => d.Donor)
                .Include(d => d.DonationRequest)
                .Where(d => !d.IsDeleted && d.DonorId == donorUserId)
                .ToListAsync();
        }

        public async Task<DonationOrder?> GetByIdAsync(Guid id)
        {
            return await _context.DonationOrders
                .AsNoTracking()
                .Include(d => d.Donor)
                .Include(d => d.DonationRequest)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<DonationOrder?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.DonationOrders
                .Include(d => d.Donor)
                .Include(d => d.DonationRequest)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task UpdateAsync(DonationOrder donationOrder)
        {
            _context.DonationOrders.Update(donationOrder);
            await _context.SaveChangesAsync();
        }
    }
}