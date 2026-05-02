using GivingChampion.Application.DTO;
using GivingChampion.Application.DTO.GeoQuestDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces
{
    public interface IGeoQuestService
    {
        Task<Result<PagedList<GeoQuestDto>>> GetAllAsync(PageParameters pageParameters);
        Task<Result<GeoQuestDto?>> GetByIdAsync(Guid id);
        Task<Result<GeoQuestDto>> CreateAsync(CreateGeoQuestDto dto);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateGeoQuestDto dto);
        Task<Result<bool>> SoftDeleteAsync(Guid id);
    }
}