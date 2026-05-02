using GivingChampion.Application.DTO.UserBadgeDto;

namespace GivingChampion.API.Interfaces
{
    public interface IUserBadgeService
    {
        Task<List<UserBadgeDto>> GetAllByProfileIdAsync(Guid profileId);
        Task<UserBadgeDto?> GetByIdAsync(Guid id);
        Task<UserBadgeDto> CreateAsync(CreateUserBadgeDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateUserBadgeDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}