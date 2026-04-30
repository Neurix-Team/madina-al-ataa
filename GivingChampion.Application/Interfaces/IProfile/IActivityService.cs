using GivingChampion.Common.DTO.ActivityDto;
using GivingChampion.Common.DTO.GivingChampion.Common.DTO.ActivityDto;

namespace GivingChampion.API.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityDto>> GetAllAsync();
        Task<ActivityDto?> GetByIdAsync(Guid id);
        Task<ActivityDto> CreateAsync(CreateActivityDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateActivityDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}