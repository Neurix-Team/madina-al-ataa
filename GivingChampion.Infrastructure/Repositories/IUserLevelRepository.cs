using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IUserLevelRepository
    {
        Task<List<UserLevel>> GetAllAsync();
        Task<UserLevel?> GetByIdAsync(Guid id);
        Task AddAsync(UserLevel userLevel);
        void Update(UserLevel userLevel);
        Task SaveChangesAsync();
    }
}