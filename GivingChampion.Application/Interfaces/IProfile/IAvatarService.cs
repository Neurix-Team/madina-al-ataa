using GivingChampion.Common.DTO.AvatarDto;

namespace GivingChampion.API.Interfaces
{
    public interface IAvatarService
    {
        Task<List<AvatarDto>> GetAllAsync();
        Task<AvatarDto?> GetByIdAsync(Guid id);
        Task<AvatarDto> CreateAsync(CreateAvatarDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateAvatarDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}