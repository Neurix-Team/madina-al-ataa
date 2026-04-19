using GivingChampion.Common.DTO.ReviewDto;

namespace GivingChampion.API.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetAllAsync();
        Task<ReviewDto?> GetByIdAsync(Guid id);
        Task<ReviewDto> CreateAsync(CreateReviewDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateReviewDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
