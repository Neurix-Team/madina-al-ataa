using GivingChampion.Common.DTO.AiAvatarDto;

namespace GivingChampion.API.Interfaces
{
    public interface IAiAvatarService
    {
        //Task<List<AiAvatarDto>> GetAllAsync();
        Task<AiAvatarDto?> GetByIdAsync(Guid id);
        Task<AiAvatarDto> CreateAsync(CreateAiAvatarDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateAiAvatarDto dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}