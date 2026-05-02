using GivingChampion.Domain.Entities;

namespace GivingChampion.Persistance.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllByProfileIdAsync(Guid profileId);
        Task<Review?> GetByIdAsync(Guid id);
        Task AddAsync(Review review);
        void Update(Review review);
    }
}
