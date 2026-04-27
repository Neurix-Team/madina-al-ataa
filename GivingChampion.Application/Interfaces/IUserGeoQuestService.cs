using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.GeoQuestDto;
using GivingChampion.Common.DTO.UserGeoQuestDto;
using GivingChampion.Common.Pagination;
using GivingChampion.Common.Results;

namespace GivingChampion.Application.Interfaces
{
    public interface IUserGeoQuestService
    {
        Task<Result<PagedList<UserGeoQuestDto>>> GetAllAsync( Guid userId,PageParameters pageParameters);
        Task<Result<UserGeoQuestDto?>> GetByIdAsync(Guid id);
        Task<Result<bool>> UpdateAsync(Guid id, UpdateUserGeoQuestDto dto , bool isSuccess = false);
        Task<Result<bool>> SoftDeleteAsync(Guid id);
        Task<Result<UserGeoQuestDto>> StartAsync(Guid geoQuestId, Guid userId);
        Task<Result<bool>> UpdateAsyncVerification(Guid id, VerifyLocationDto dto, bool isSuccess = false);
        Task<Result<UserGeoQuestDto>> CheckGeoQuestStatus(Guid id);

    }
}