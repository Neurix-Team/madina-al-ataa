using GivingChampion.Common.DTO.VolunteerOrder;

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

        Task<VolunteerOrderDto?> ApproveOrderAsync(Guid id);
        // Added string rejectionReason to match business logic
        Task<VolunteerOrderDto?> RejectOrderAsync(Guid id, string rejectionReason);

        #region Command Methods
        Task<VolunteerOrderDto> CreateAsync(CreateVolunteerOrderDto dto, Guid volunteerId);
        Task<VolunteerOrderDto?> UpdateAsync(Guid id, UpdateVolunteerOrderDto dto);
        Task<bool> DeleteAsync(Guid id);
        #endregion
    }
}