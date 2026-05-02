using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
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
    public class DonationRequestRepository : IDonationRequestRepository
    {
        private readonly AppDbContext _context;
   
        public DonationRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DonationRequest donationRequest)
        {
            await _context.DonationRequests.AddAsync(donationRequest);
        }

        public Task DeleteAsync(DonationRequest donationRequest)
        {
            _context.DonationRequests.Remove(donationRequest);
            return Task.CompletedTask;
        }

        public async Task<PagedList<DonationRequest>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.DonationRequests
                .AsNoTracking()
                .Where(dr => !dr.IsDeleted).ToPagedListAsync(pageParameters);
    }

        public async Task<PagedList<DonationRequest>> GetApprovedAsync(PageParameters pageParameters)
        {
            return await _context.DonationRequests
                .AsNoTracking()
                .Where(dr =>
                    !dr.IsDeleted && dr.Status == RequestStatus.Approved)
                .ToPagedListAsync(pageParameters);
    }
  
        //public async Task<IEnumerable<DonationRequest>> GetByParentIdAsync(string parentUserId)
        //{
        //    return await _context.DonationRequests
        //        .AsNoTracking()
        //        .Where(dr =>
        //            !dr.IsDeleted)
        //        .ToListAsync();
        //}

        public async Task<DonationRequest?> GetByIdAsync(Guid id)
        {
            return await _context.DonationRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(dr => dr.Id == id && !dr.IsDeleted);
        }

        public async Task<PagedList<DonationRequest>> GetRequestsByUserAsync(Guid userId, PageParameters pageParameters)
        {
            var requestIds = await _context.DonationOrders
                .AsNoTracking()
                .Where(o =>
                    o.DonorId == userId &&
                    !o.IsDeleted)
                .Select(o => o.DonationRequestId)
                .Distinct()
                .ToListAsync();

            return await _context.DonationRequests
                .AsNoTracking()
                .Where(r =>
                    requestIds.Contains(r.Id) &&
                    !r.IsDeleted)
                .ToPagedListAsync(pageParameters);
        }

        public Task UpdateAsync(DonationRequest donationRequest)
        {
            _context.DonationRequests.Update(donationRequest);
            return Task.CompletedTask;
        }
    }
}
