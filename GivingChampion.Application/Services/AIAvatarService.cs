using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Common.DTO.AiAvatarDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class AiAvatarService : IAiAvatarService
    {
        private readonly IAiAvatarRepository _aiAvatarRepository;
        private readonly IMapper _mapper;

        public AiAvatarService(IAiAvatarRepository aiAvatarRepository, IMapper mapper)
        {
            _aiAvatarRepository = aiAvatarRepository;
            _mapper = mapper;
        }

        //public async Task<List<AiAvatarDto>> GetAllAsync()
        //{
        //    var aiAvatars = await _aiAvatarRepository.GetAllAsync();
        //    return _mapper.Map<List<AiAvatarDto>>(aiAvatars);
        //}

        public async Task<AiAvatarDto?> GetByIdAsync(Guid id)
        {
            var aiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (aiAvatar == null)
                return null;

            return _mapper.Map<AiAvatarDto>(aiAvatar);
        }

        public async Task<AiAvatarDto> CreateAsync(CreateAiAvatarDto dto)
        {
            var aiAvatar = _mapper.Map<AiAvatar>(dto);
            aiAvatar.Id = Guid.NewGuid();
            await _aiAvatarRepository.AddAsync(aiAvatar);
            await _aiAvatarRepository.SaveChangesAsync();

            return _mapper.Map<AiAvatarDto>(aiAvatar);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAiAvatarDto dto)
        {
            var existingAiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (existingAiAvatar == null)
                return false;

            _mapper.Map(dto, existingAiAvatar);
            _aiAvatarRepository.Update(existingAiAvatar);
            await _aiAvatarRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var aiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (aiAvatar == null)
                return false;

            aiAvatar.IsDeleted = true;
            aiAvatar.DeletedAt = DateTime.UtcNow;

            _aiAvatarRepository.Update(aiAvatar);
            await _aiAvatarRepository.SaveChangesAsync();

            return true;
        }
    }
}