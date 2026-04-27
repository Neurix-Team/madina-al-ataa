using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IVolunteerOrderRepository
    {
        Task<List<VolunteerOrder>> GetAllAsync();
        Task<VolunteerOrder?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        Task<VolunteerOrder> CreateAsync(VolunteerOrder volunteerOrder);

        Task UpdateAsync(VolunteerOrder volunteerOrder);
        Task SoftDeleteAsync(VolunteerOrder volunteerOrder);

        Task SaveChangesAsync();
    }
}