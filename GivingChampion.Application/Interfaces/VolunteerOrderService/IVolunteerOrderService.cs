using GivingChampion.Common.DTO.VolunteerOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace GivingChampion.Application.Interfaces.VolunteerOrderService
{
         public interface IVolunteerOrderService { 

            #region Query Methods

            // Gets all volunteer orders that are not soft deleted
            Task<List<VolunteerOrderDto>> GetAllAsync();

            // Gets a single volunteer order by its id
            // Returns null if the order does not exist or is soft deleted
            Task<VolunteerOrderDto?> GetByIdAsync(Guid id);
        #endregion

            #endregion

            #region Command Methods

            // Creates a new volunteer order
            // VolunteerId is taken from the authenticated user's JWT token
            Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto, Guid volunteerId);

            // Updates an existing volunteer order by its id
            // Returns null if the order does not exist or is soft deleted
            Task<VolunteerOrderDto?> UpdateAsync(Guid id, UpdateVolunteerOrderDto dto);

            // Soft deletes a volunteer order by its id
            // Returns true if deleted successfully, otherwise false
            Task<bool> DeleteAsync(Guid id);

            #endregion
        }
    }

