using GivingChampion.Domain.Entities;

namespace GivingChampion.API.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(Guid id);
        Task AddAsync(Review review);
        void Update(Review review);
        Task SaveChangesAsync();
    }
}
