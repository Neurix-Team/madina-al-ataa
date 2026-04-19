using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerRepository
    {
        #region Query Methods

        Task<List<Volunteer>> GetAllAsync(); // Return List<Volunteer> (Entities)
        Task<Volunteer?> GetByIdAsync(Guid id); // Return Volunteer (Entity)

        #endregion

        #region Command Methods

        Task AddAsync(Volunteer volunteer); // Add a new volunteer (Entity)
        void Update(Volunteer volunteer); // Update existing volunteer (Entity)
        Task SaveChangesAsync(); // Save changes to the database

        #endregion
    }
}