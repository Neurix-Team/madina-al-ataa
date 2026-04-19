using GivingChampion.Common.DTO.UserBadgeDto;

namespace GivingChampion.API.Interfaces
{
    public interface IUserBadgeService
    {
        Task<List<UserBadgeDto>> GetAllAsync();
        Task<UserBadgeDto?> GetByIdAsync(Guid id);
        Task<UserBadgeDto> CreateAsync(CreateUserBadgeDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateUserBadgeDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}