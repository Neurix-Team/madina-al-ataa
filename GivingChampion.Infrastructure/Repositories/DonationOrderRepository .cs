using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
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
        }

        public async Task<PagedList<DonationOrder>> GetAllAsync(PageParameters pageParameters)
        {
            var query = await _context.DonationOrders
                .AsNoTracking()
                .Include(d => d.Donor)
                .Include(d => d.DonationRequest)
                .Where(d => !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt).ToPagedListAsync(pageParameters);

            return query;
        }

        public async Task<PagedList<DonationOrder>> GetByDonorIdAsync(
            Guid donorUserId,
            PageParameters pageParameters)
        {
            var query = await _context.DonationOrders
                .AsNoTracking()
                                 .Include(d => d.Donor)
                                 .Include(d => d.DonationRequest)
                .Where(d =>
                    !d.IsDeleted &&
                    d.DonorId == donorUserId)
                .OrderByDescending(d => d.CreatedAt).ToPagedListAsync(pageParameters);

            return query;
        }

        // Implement GetByIdAsync method
        public async Task<DonationOrder?> GetByIdAsync(Guid id)
        {
            return await _context.DonationOrders
                .AsNoTracking()
                                 .Include(d => d.Donor)
                .FirstOrDefaultAsync(d =>
                    d.Id == id &&
                    !d.IsDeleted);
        }

        public async Task<DonationOrder?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.DonationOrders
                .Include(d => d.Donor)
                                 .Include(d => d.DonationRequest)
                .FirstOrDefaultAsync(d =>
                    d.Id == id &&
                    !d.IsDeleted);
        }

        // Implement UpdateAsync method
        public Task UpdateAsync(DonationOrder donationOrder)
        {
            _context.DonationOrders.Update(donationOrder);
            return Task.CompletedTask;
        }

        //private static async Task<PagedList<DonationOrder>> CreatePagedListAsync(
        //    IQueryable<DonationOrder> query,
        //    PageParameters pageParameters)
        //{
        //    var pageNumber = pageParameters.PageNumber <= 0
        //        ? 1
        //        : pageParameters.PageNumber;

        //    var pageSize = pageParameters.PageSize <= 0
        //        ? 10
        //        : pageParameters.PageSize;

        //    var totalCount = await query.CountAsync();

        //    var items = await query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return new PagedList<DonationOrder>(
        //        items,
        //        totalCount,
        //        pageNumber,
        //        pageSize
        //    );
        //}
    }
}
