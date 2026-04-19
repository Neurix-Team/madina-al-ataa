using GivingChampion.Common.DTO.BadgeDto;

namespace GivingChampion.API.Interfaces
{
    public interface IBadgeService
    {
        Task<List<BadgeDto>> GetAllAsync();
        Task<BadgeDto?> GetByIdAsync(Guid id);
        Task<BadgeDto> CreateAsync(CreateBadgeDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateBadgeDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}