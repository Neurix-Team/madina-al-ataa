using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IBadgeRepository
    {
        Task<PagedList<Badge>> GetAllAsync(PageParameters pageParameters);
        Task<Badge?> GetByIdAsync(Guid id);
        Task AddAsync(Badge badge);
        void Update(Badge badge);
        Task SaveChangesAsync();
    }
}