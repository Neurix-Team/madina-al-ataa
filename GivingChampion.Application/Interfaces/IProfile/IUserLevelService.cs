using GivingChampion.Common.DTO.UserLevelDto;

namespace GivingChampion.API.Interfaces
{
    public interface IUserLevelService
    {
        Task<UserLevelDto?> GetByProfileIdAsync(Guid profileId);
        //Task<UserLevelDto> CreateAsync(CreateUserLevelDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateUserLevelDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}