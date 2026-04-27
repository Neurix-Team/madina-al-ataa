using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IDonationOrderRepository
    {
        Task CreateAsync(DonationOrder donationOrder);
        Task<PagedList<DonationOrder>> GetAllAsync(PageParameters pageParameters);
        Task<PagedList<DonationOrder>> GetByDonorIdAsync(
            Guid donorUserId,
            PageParameters pageParameters);
        Task<DonationOrder?> GetByIdAsync(Guid id);
        Task UpdateAsync(DonationOrder donationOrder);
    }
}