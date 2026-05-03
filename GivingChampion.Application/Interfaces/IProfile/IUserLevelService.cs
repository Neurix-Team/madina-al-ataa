using GivingChampion.Application.DTO.UserLevelDto;

namespace GivingChampion.API.Interfaces
{
    public interface IUserLevelService
    {
        Task<UserLevelDto?> GetByProfileIdAsync(Guid profileId);
        Task<bool> UpdateAsync(Guid id, UpdateUserLevelDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}