using GivingChampion.Common.Enums;
using GivingChampion.Domain.Contexts;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GivingChampion.Persistance.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        #region Fields

        // Database context used to access ServiceRequests table
        private readonly AppDbContext _context;

        #endregion

        #region Constructor

        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Get Methods

        // Gets all service requests that are not soft deleted
        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Partner)
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        // Gets a single service request by id if it is not soft deleted
        public async Task<ServiceRequest?> GetByIdAsync(Guid id)
        {
            return await _context.ServiceRequests
                .Include(x => x.Partner)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        // Gets all pending service requests that are not soft deleted
        public async Task<List<ServiceRequest>> GetPendingAsync()
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Partner)
                .Where(x => !x.IsDeleted && x.Status == RequestStatus.Pending)
                .ToListAsync();
        }

        // Gets all service requests related to a specific partner
        public async Task<List<ServiceRequest>> GetByPartnerIdAsync(Guid partnerId)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Partner)
                .Where(x => x.PartnerId == partnerId && !x.IsDeleted)
                .ToListAsync();
        }

        #endregion

        #region Create Method

        // Adds a new service request to the DbContext
        // Note: This does not save to database until SaveChangesAsync is called
        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);
        }

        #endregion

        #region Update Method

        // Marks an existing service request as modified
        // Note: This does not save to database until SaveChangesAsync is called
        public void Update(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
        }

        #endregion

        #region Delete Method

        // Soft deletes a service request instead of removing it from database
        // This keeps the record but marks it as deleted
        public void SoftDelete(ServiceRequest serviceRequest)
        {
            serviceRequest.IsDeleted = true;
            serviceRequest.DeletedAt = DateTime.UtcNow;

            _context.ServiceRequests.Update(serviceRequest);
        }

        #endregion

        #region Save Changes

        // Saves all tracked changes to the database
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}