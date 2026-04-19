using GivingChampion.Common.DTO.LevelDto;

namespace GivingChampion.API.Interfaces
{
    public interface ILevelService
    {
        Task<List<LevelDto>> GetAllAsync();
        Task<LevelDto?> GetByIdAsync(Guid id);
        Task<LevelDto> CreateAsync(CreateLevelDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateLevelDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}