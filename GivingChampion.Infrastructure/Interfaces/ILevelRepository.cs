using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface ILevelRepository
    {
        Task<PagedList<Level>> GetAllAsync(PageParameters pageParameters);
        Task<Level?> GetByIdAsync(Guid id);
        Task AddAsync(Level level);
        void Update(Level level);
        Task SaveChangesAsync();
    }
}