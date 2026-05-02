using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerOrderRepository
    {
        Task<List<VolunteerOrder>> GetAllAsync();
        Task<VolunteerOrder?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        Task AddAsync(VolunteerOrder volunteerOrder);
        void Update(VolunteerOrder volunteerOrder);
        Task<bool> ExistsActiveByUserAndServiceRequestAsync(Guid userId, Guid serviceRequestId);
    }
}
