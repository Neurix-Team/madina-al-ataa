using GivingChampion.Common.DTO.LevelDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.API.Interfaces
{
    public interface ILevelService
    {
        Task<Result<PagedList<LevelDto>>> GetAllAsync(PageParameters pageParameters);
        Task<Result<LevelDto?>> GetByIdAsync(Guid id);
        Task<Result<LevelDto>> CreateAsync(CreateLevelDto dto);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateLevelDto dto);
        Task<Result<bool>> SoftDeleteAsync(Guid id);
    }
}