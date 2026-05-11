using GivingChampion.Common.Enums;
using GivingChampion.Common.Extensions.Pagination;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistence.Contexts;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly AppDbContext _context;

        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<ServiceRequest>> GetAllAsync(PageParameters pageParameters)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(sr => sr.RequiredLevel)
                .Include(sr => sr.Partner)
                .Where(sr => !sr.IsDeleted)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToPagedListAsync(pageParameters);
        }

        public async Task<ServiceRequest?> GetByIdAsync(Guid id)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(sr => sr.RequiredLevel)
                .Include(sr => sr.Partner)
                .FirstOrDefaultAsync(sr => sr.Id == id && !sr.IsDeleted);
        }

        public async Task<PagedList<ServiceRequest>> GetByStatusAsync(
            RequestStatus status,
            PageParameters pageParameters)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(sr => sr.RequiredLevel)
                .Include(sr => sr.Partner)
                .Where(sr =>
                    sr.Status == status &&
                    !sr.IsDeleted)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToPagedListAsync(pageParameters);
        }

        public async Task<List<ServiceRequest>> GetApprovedRequestsAsync()
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(sr => sr.RequiredLevel)
                .Include(sr => sr.Partner)
                .Where(sr =>
                    sr.Status == RequestStatus.Approved &&
                    !sr.IsDeleted)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ServiceRequest>> GetByPartnerIdAsync(Guid partnerId)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(sr => sr.RequiredLevel)
                .Include(sr => sr.Partner)
                .Where(sr =>
                    sr.PartnerId == partnerId &&
                    !sr.IsDeleted)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdForUpdateAsync(Guid id)
        {
            return await _context.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.Id == id && !sr.IsDeleted);
        }

        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);
        }

        public Task UpdateAsync(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
            return Task.CompletedTask;
        }

        public Task SoftDeleteAsync(ServiceRequest serviceRequest)
        {
            serviceRequest.IsDeleted = true;
            serviceRequest.DeletedAt = DateTime.UtcNow;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            _context.ServiceRequests.Update(serviceRequest);

            return Task.CompletedTask;
        }

        public async Task UpdateProgressAsync(Guid serviceRequestId, int progress)
        {
            var serviceRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequestId && !sr.IsDeleted);

            if (serviceRequest == null)
                return;

            serviceRequest.Progress = progress;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            _context.ServiceRequests.Update(serviceRequest);
        }
    }
}