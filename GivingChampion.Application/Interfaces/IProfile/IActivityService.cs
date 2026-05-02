using GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.DTO.GivingChampion.Application.DTO.ActivityDto;

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