using GivingChampion.Application.DTO.ProfileDto;
using GivingChampion.Common.Results;

namespace GivingChampion.API.Interfaces
{
    public interface IProfileService
    {
        Task<Result<ProfileDto?>> GetByIdAsync(Guid id);
        Task<Result<ProfileDto?>> GetByUserIdAsync();

        //Task<ProfileDto> CreateAsync(CreateProfileDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateProfileDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
