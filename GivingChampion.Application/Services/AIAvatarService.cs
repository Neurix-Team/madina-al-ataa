using AutoMapper;
using GivingChampion.API.Interfaces;
using GivingChampion.Application.Exceptions;
using GivingChampion.Common.DTO.AiAvatarDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.API.Services
{
    public class AiAvatarService : IAiAvatarService
    {
        private readonly IAiAvatarRepository _aiAvatarRepository;
        private readonly IMapper _mapper;

        public AiAvatarService(
            IAiAvatarRepository aiAvatarRepository,
            IMapper mapper)
        {
            _aiAvatarRepository = aiAvatarRepository;
            _mapper = mapper;
        }

        public async Task<AiAvatarDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("AI Avatar ID is required.");

            var aiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (aiAvatar == null)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            if (aiAvatar.IsDeleted)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            return _mapper.Map<AiAvatarDto>(aiAvatar);
        }

        public async Task<AiAvatarDto> CreateAsync(CreateAiAvatarDto dto)
        {
            if (dto == null)
                throw new BadRequestException("AI Avatar create data is required.");

            var aiAvatar = _mapper.Map<AiAvatar>(dto);

            await _aiAvatarRepository.AddAsync(aiAvatar);
            await _aiAvatarRepository.SaveChangesAsync();

            return _mapper.Map<AiAvatarDto>(aiAvatar);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAiAvatarDto dto)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("AI Avatar ID is required.");

            if (dto == null)
                throw new BadRequestException("AI Avatar update data is required.");

            var existingAiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (existingAiAvatar == null)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            if (existingAiAvatar.IsDeleted)
                throw new BadRequestException("Cannot update a deleted AI Avatar.");

            _mapper.Map(dto, existingAiAvatar);

            _aiAvatarRepository.Update(existingAiAvatar);

            await _aiAvatarRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new BadRequestException("AI Avatar ID is required.");

            var aiAvatar = await _aiAvatarRepository.GetByIdAsync(id);

            if (aiAvatar == null)
                throw new NotFoundException($"AI Avatar with ID {id} was not found.");

            if (aiAvatar.IsDeleted)
                throw new BadRequestException("AI Avatar is already deleted.");

            aiAvatar.IsDeleted = true;
            aiAvatar.DeletedAt = DateTime.UtcNow;

            _aiAvatarRepository.Update(aiAvatar);

            await _aiAvatarRepository.SaveChangesAsync();

            return true;
        }
    }
}