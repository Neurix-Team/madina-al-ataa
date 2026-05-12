using GivingChampion.Application.DTO.UserLevelDto;

namespace GivingChampion.API.Interfaces
{
    public interface IUserLevelService
    {
        Task<UserLevelDto?> GetMyLevelAsync();
        Task<UserLevelDto?> GetByUserIdAsync(Guid userId);
        Task<bool> UpdateAsync(Guid id, UpdateUserLevelDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}