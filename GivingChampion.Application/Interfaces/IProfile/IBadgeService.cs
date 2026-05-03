using GivingChampion.Application.DTO.BadgeDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.API.Interfaces
{
    public interface IBadgeService
    {
        Task<Result<PagedList<BadgeDto>>> GetAllAsync(PageParameters pageParameters);
        Task<Result<BadgeDto?>> GetByIdAsync(Guid id);
        Task<Result<BadgeDto>> CreateAsync(CreateBadgeDto dto);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateBadgeDto dto);
        Task<Result<bool>> SoftDeleteAsync(Guid id);
    }
}