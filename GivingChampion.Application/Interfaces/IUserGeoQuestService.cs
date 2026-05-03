using GivingChampion.Application.DTO;
using GivingChampion.Application.DTO.GeoQuestDto;
using GivingChampion.Application.DTO.UserGeoQuestDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces
{
    public interface IUserGeoQuestService
    {
        Task<Result<PagedList<UserGeoQuestDto>>> GetAllAsync(
    PageParameters pageParameters);
        Task<Result<UserGeoQuestDto?>> GetByIdAsync(Guid id);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateUserGeoQuestDto dto , bool isSuccess = false);
        Task<Result<bool>> SoftDeleteAsync(Guid id);
        Task<Result<UserGeoQuestDto>> StartAsync(Guid geoQuestId);
        Task<Result<string>> UpdateAsyncVerification(VerifyLocationDto dto);
        Task<Result<UserGeoQuestDto>> CheckGeoQuestStatus(Guid id);

    }
}
