using GivingChampion.Common.DTO.Location;

namespace GivingChampion.Application.Interfaces.Location
{
    public interface ILocationService
    {
        Task<Result<List<LocationDto>>> GetAllAsync();
        Task<Result<List<LocationDto>>> GetAvailableForUserAsync(int userLevel);
        Task<Result<LocationDto>> GetByIdAsync(Guid id);
        Task<Result<LocationDto>> CreateLocationAsync(CreateLocationDto dto);
        Task<Result> UpdateLocationAsync(Guid id, UpdateLocationDto dto);
        Task<Result> SoftDeleteLocationAsync(Guid id);
    }
}