using GivingChampion.Common.DTO.VolunteerDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace GivingChampion.Application.Interfaces.Volunteer
{
    public interface IVolunteerService
    {


        #region Query Methods

        #region GetAllVolunteers

        /// <summary>
        /// Gets all volunteers and returns them as DTOs.
        /// </summary>
        Task<List<VolunteerDto>> GetAllAsync();

        #endregion

        #region GetVolunteerById

        /// <summary>
        /// Gets a single volunteer by id.
        /// Returns null if the volunteer does not exist.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        Task<VolunteerDto?> GetByIdAsync(Guid id);

        #endregion

        #endregion

        #region Command Methods

        #region CreateVolunteer

        /// <summary>
        /// Creates a new volunteer.
        /// This operation should be allowed for Admin only in the Controller.
        /// </summary>
        /// <param name="dto">Volunteer creation data.</param>
        Task<VolunteerDto> CreateAsync(CreateVolunteerDto dto);

        #endregion

        #region UpdateVolunteer

        /// <summary>
        /// Updates an existing volunteer.
        /// Returns null if the volunteer does not exist.
        /// This operation should be allowed for Admin only in the Controller.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <param name="dto">Volunteer update data.</param>
        Task<VolunteerDto?> UpdateAsync(Guid id, UpdateVolunteerDto dto);

        #endregion

        #endregion
    }
}

