using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerOrderRepository
    {
        #region Query Methods

        // Gets all volunteer orders that are not soft deleted
        Task<List<VolunteerOrder>> GetAllAsync();

        // Gets a single volunteer order by its id
        // Returns null if the order does not exist or is soft deleted
        Task<VolunteerOrder?> GetByIdAsync(Guid id);

        // Checks whether a volunteer order exists and is not soft deleted
        Task<bool> ExistsAsync(Guid id);

        #endregion

        #region Command Methods

        // Adds a new volunteer order to the DbContext
        // Note: This does not save changes to the database until SaveChangesAsync is called
        Task AddAsync(VolunteerOrder volunteerOrder);

        // Marks an existing volunteer order as modified
        // Note: This does not save changes to the database until SaveChangesAsync is called
        void Update(VolunteerOrder volunteerOrder);

        // Soft deletes a volunteer order instead of physically removing it from the database
        // Usually sets IsDeleted = true and DeletedAt = current date/time
        void SoftDelete(VolunteerOrder volunteerOrder);

        // Saves all pending changes to the database
        Task SaveChangesAsync();

        #endregion
    }
}