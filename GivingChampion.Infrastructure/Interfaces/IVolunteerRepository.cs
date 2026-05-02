using GivingChampion.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerRepository
    {
        #region Query Methods

        //Task<List<Volunteer>> GetAllAsync(); // Return List<Volunteer> (Entities)
        Task<Volunteer?> GetByIdAsync(Guid id); // Return Volunteer (Entity)
        Task<Volunteer?> GetByUserIdAsync(Guid userId);

        #endregion

        #region Command Methods

        Task AddAsync(Guid userId); // Add a new volunteer (Entity)
        //void Update(Volunteer volunteer); // Update existing volunteer (Entity)

        #endregion
    }
}
