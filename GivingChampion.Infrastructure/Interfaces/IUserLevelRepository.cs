using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IUserLevelRepository
    {
        Task<List<UserLevel>> GetAllAsync();
        Task<UserLevel?> GetByProfileIdAsync(Guid profileId);
        Task<UserLevel?> GetByIdAsync(Guid id);
        Task AddAsync(UserLevel userLevel);
        void Update(UserLevel userLevel);
        Task SaveChangesAsync();
    }
}