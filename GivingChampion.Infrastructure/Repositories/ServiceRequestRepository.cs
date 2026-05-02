using GivingChampion.Common.Enums;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
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

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Partner)
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(Guid id)
        {
            return await _context.ServiceRequests
                .Include(x => x.Partner)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public async Task<List<ServiceRequest>> GetApprovedRequestsAsync()
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Partner)
                .Where(sr => sr.Status == RequestStatus.Approved && !sr.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<ServiceRequest>> GetByPartnerIdAsync(Guid partnerId)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Partner)
                .Where(x => x.PartnerId == partnerId && !x.IsDeleted)
                .ToListAsync();
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
            //serviceRequest.IsDeleted = true;
            //serviceRequest.DeletedAt = DateTime.UtcNow;

            //_context.ServiceRequests.Update(serviceRequest);
            _context.ServiceRequests.Remove(serviceRequest);

            return Task.CompletedTask;
        }

        public async Task UpdateProgressAsync(Guid serviceRequestId, int progress)
        {
            var serviceRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(x => x.Id == serviceRequestId && !x.IsDeleted);

            if (serviceRequest == null)
                return;

            serviceRequest.Progress = progress;

            _context.ServiceRequests.Update(serviceRequest);
        }
    }
}
