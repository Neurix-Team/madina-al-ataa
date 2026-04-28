using GivingChampion.Common.DTO.ProfileDto;

namespace GivingChampion.API.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetByIdAsync(Guid id);
        Task<ProfileDto> CreateAsync(CreateProfileDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateProfileDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}