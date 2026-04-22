using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GivingChampion.Persistence.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        private readonly AppDbContext _context;

        public DonorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Donor?> GetByIdAsync(Guid id)
        {
            return await _context.Donors
                .IgnoreQueryFilters()           // In case we need to access soft-deleted
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Donor?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == userId && !d.IsDeleted);
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.Donors
                .AnyAsync(d => d.UserId == userId && !d.IsDeleted);
        }

        public async Task CreateAsync(Donor donor)
        {
            await _context.Donors.AddAsync(donor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Donor donor)
        {
            _context.Donors.Update(donor);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Soft deletes the Donor profile when the main user is deleted
        /// </summary>
        public async Task SoftDeleteAsync(Guid userId)
        {
            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == userId && !d.IsDeleted);

            if (donor == null) return;

            _context.Donors.Remove(donor);

            await _context.SaveChangesAsync();
        }
    }
}