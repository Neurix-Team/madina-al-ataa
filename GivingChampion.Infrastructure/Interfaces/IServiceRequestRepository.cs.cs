using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IServiceRequestRepository
    {
        // Gets all service requests that are not soft deleted
        Task<List<ServiceRequest>> GetAllAsync();

        // Gets a single service request by its id
        // Returns null if the request does not exist or is soft deleted
        Task<ServiceRequest?> GetByIdAsync(Guid id);

        // Gets all service requests with Pending status
        //Task<List<ServiceRequest>> GetPendingAsync();
        Task<List<ServiceRequest>> GetApprovedRequestsAsync();


        // Gets all service requests created by or related to a specific partner
        Task<List<ServiceRequest>> GetByPartnerIdAsync(Guid partnerId);

        // Adds a new service request to the DbContext
        // Note: This does not save changes to the database until IUnitOfWork.SaveChangesAsync is called
        Task AddAsync(ServiceRequest serviceRequest);

        // Marks an existing service request as modified
        // Note: This does not save changes to the database until IUnitOfWork.SaveChangesAsync is called
        Task UpdateAsync(ServiceRequest serviceRequest);

        // Soft deletes a service request instead of physically removing it from the database
        // Usually sets IsDeleted = true and DeletedAt = current date/time
        Task SoftDeleteAsync(ServiceRequest serviceRequest);
        Task UpdateProgressAsync(Guid serviceRequestId, int progress);
        // Saves all pending changes to the database
    }
}
